using MicroCashDevWPFUI.Data;
using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.ProdukService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Services.PembelianService
{
	public class RestokService : IRestokService
	{
		private readonly IAppDbContext _context;
		private readonly IProdukSatuanService _produkSatuanService;
		private readonly IProdukBatchService _batchService;
		private readonly IPembelianService _pembelianService;
		private readonly IDetailPembelianService _detailPembelianService;

		public RestokService(
			IAppDbContext context,
			IProdukSatuanService produkSatuanService,
			IProdukBatchService batchService,
			IPembelianService pembelianService,
			IDetailPembelianService detailPembelianService)
		{
			_context = context;
			_produkSatuanService = produkSatuanService;
			_batchService = batchService;
			_pembelianService = pembelianService;
			_detailPembelianService = detailPembelianService;
		}

		public async Task RestokProdukAsync( int produkSatuanId, int jumlahInput, DateTime tanggalKadaluarsa, decimal hargaBeli)
		{
			if (jumlahInput <= 0)
				throw new InvalidOperationException("Jumlah harus > 0");

			using var tx = await _context.BeginTransactionAsync();

			try
			{
				// 1️⃣ SATUAN YANG DIPILIH USER
				var satuanInput = await _produkSatuanService.GetByIdAsync(produkSatuanId)
					?? throw new InvalidOperationException("Satuan tidak ditemukan");

				// 2️⃣ SEMUA SATUAN PRODUK
				var semuaSatuan = await _produkSatuanService.GetByProdukIdAsync(satuanInput.ProdukId);

				// 3️⃣ SATUAN DASAR (BUTIR = JumlahPerSatuan == 1)
				var satuanDasar = semuaSatuan
					.FirstOrDefault(s => s.JumlahPerSatuan == 1)
					?? throw new InvalidOperationException(
						"Satuan dasar tidak ditemukan. Periksa data ProdukSatuan.");

				// 4️⃣ KONVERSI KE BUTIR (BENAR)
				int stokButir = jumlahInput * satuanInput.JumlahPerSatuan;

				// 5️⃣ PEMBELIAN
				var supplier = await _context.Suppliers
					.FirstAsync(s => s.NamaSupplier == "Restok Manual");

				var pembelian = await _pembelianService.CreatePembelianAsync(new Pembelian
				{
					SupplierId = supplier.Id,
					Tanggal = DateTime.Now,
					Total = jumlahInput * hargaBeli
				});

				// 6️⃣ DETAIL PEMBELIAN (SATUAN ASLI USER)
				var detail = new DetailPembelian
				{
					PembelianId = pembelian.Id,
					ProdukSatuanId = satuanInput.Id,
					Jumlah = jumlahInput,
					HargaBeli = hargaBeli,
					Subtotal = jumlahInput * hargaBeli
				};

				await _detailPembelianService.AddAsync(detail);

				// 7️⃣ BATCH → SELALU BUTIR
				var batch = new ProdukBatch
				{
					ProdukSatuanId = satuanDasar.Id,
					DetailPembelianId = detail.Id,
					Stok = stokButir,
					TanggalMasuk = DateTime.Now,
					TanggalKadarluasa = tanggalKadaluarsa
				};

				await _batchService.AddAsync(batch);

				await tx.CommitAsync();
			}
			catch
			{
				await tx.RollbackAsync();
				throw;
			}
		}

		public async Task RestokNotaAsync(int supplierId,
								  List<RestokNotaItemDto> items,
								  decimal totalNota,
								  string? nomorFaktur,
								  DateTime tanggalRestok)
		{
			if (!items.Any())
				throw new InvalidOperationException("Item kosong");

			using var tx = await _context.BeginTransactionAsync();

			try
			{
				// 1️⃣ SUPPLIER
				var supplier = await _context.Suppliers
					.FirstOrDefaultAsync(s => s.Id == supplierId)
					?? throw new InvalidOperationException("Supplier tidak ditemukan");

				// 2️⃣ PEMBELIAN (HEADER)
				var pembelian = await _pembelianService.CreatePembelianAsync(new Pembelian
				{
					SupplierId = supplier.Id,
					Tanggal = tanggalRestok,   // ⬅ PAKAI YANG DARI UI
					NomorFaktur = nomorFaktur?.Trim() ?? string.Empty,
					Total = totalNota
				});

				// 3️⃣ LOOP ITEM
				foreach (var item in items)
				{
					var satuanInput = await _produkSatuanService.GetByIdAsync(item.ProdukSatuanId)
						?? throw new InvalidOperationException("Satuan tidak ditemukan");

					var semuaSatuan = await _produkSatuanService.GetByProdukIdAsync(satuanInput.ProdukId);

					var satuanDasar = semuaSatuan
						.First(s => s.JumlahPerSatuan == 1);

					int stokButir = item.Jumlah * satuanInput.JumlahPerSatuan;

					// 4️⃣ DETAIL PEMBELIAN
					var detail = new DetailPembelian
					{
						PembelianId = pembelian.Id,
						ProdukSatuanId = satuanInput.Id,
						Jumlah = item.Jumlah,
						HargaBeli = item.HargaBeli,
						Subtotal = item.SubTotal
					};

					await _detailPembelianService.AddAsync(detail);

					// 5️⃣ BATCH
					var batch = new ProdukBatch
					{
						ProdukSatuanId = satuanDasar.Id,
						DetailPembelianId = detail.Id,
						Stok = stokButir,
						TanggalMasuk = item.TanggalMasuk,
						TanggalKadarluasa = item.TanggalKadarluasa,
						BatchNumber = item.NomorBatch
					};

					await _batchService.AddAsync(batch);
				}

				await tx.CommitAsync();
			}
			catch
			{
				await tx.RollbackAsync();
				throw;
			}
		}


	}
}
