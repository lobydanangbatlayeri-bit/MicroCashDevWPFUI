using MicroCashDevWPFUI.Data;
using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroCashDevWPFUI.Services.PenjualanService
{
	public class PenjualanService : IPenjualanService
	{
		private readonly IAppDbContext _context;

		public PenjualanService(IAppDbContext context)
		{
			_context = context;
		}

		public async Task<Penjualan> ProsesPenjualanAsync(
			int userId,
			decimal dibayar,
			IEnumerable<KeranjangItems> keranjang)
		{
			if (!keranjang.Any())
				throw new Exception("Keranjang kosong.");

			using var trx = await _context.BeginTransactionAsync();

			try
			{
				var penjualan = CreatePenjualan(userId, dibayar, keranjang);
				_context.Penjualans.Add(penjualan);
				await _context.SaveChangesAsync();

				var satuanDasarCache = new Dictionary<int, ProdukSatuan>();

				foreach (var item in keranjang)
				{
					await ProsesItemPenjualanAsync(
						penjualan.Id,
						item,
						satuanDasarCache
					);
				}

				await _context.SaveChangesAsync();
				await trx.CommitAsync();

				return penjualan;
			}
			catch
			{
				await trx.RollbackAsync();
				throw;
			}
		}

		public async Task<Penjualan?> GetByIdAsync(int id)
		{
			return await _context.Penjualans
				.Include(p => p.User)
				.Include(p => p.DetailPenjualans)
				.FirstOrDefaultAsync(p => p.Id == id);
		}

		public async Task<List<Penjualan>> GetRiwayatAsync(DateTime? start = null, DateTime? end = null)
		{
			var query = _context.Penjualans
				.AsNoTracking()
				.IgnoreQueryFilters()
				.Include(p => p.User)
				.Include(p => p.DetailPenjualans)
					.ThenInclude(d => d.ProdukSatuan)
						.ThenInclude(ps => ps.Produk)
				.Include(p => p.DetailPenjualans)
					.ThenInclude(d => d.ProdukSatuan)
						.ThenInclude(ps => ps.Satuan)
				.Include(p => p.DetailPenjualans)
					.ThenInclude(d => d.DetailPenjualanBatches)
				.AsQueryable();

			if (start != null)
				query = query.Where(x => x.Tanggal >= start);

			if (end != null)
				query = query.Where(x => x.Tanggal <= end);

			return await query
				.OrderByDescending(x => x.Tanggal)
				.ToListAsync();
		}

		public async Task UpdateAsync(Penjualan penjualan)
		{
			var tracked = await _context.Penjualans
				.Include(p => p.DetailPenjualans)
				.FirstOrDefaultAsync(p => p.Id == penjualan.Id);

			if (tracked == null)
				throw new InvalidOperationException(
					$"Penjualan dengan Id={penjualan.Id} tidak ditemukan.");

			tracked.Tanggal = penjualan.Tanggal;
			tracked.NomorNota = penjualan.NomorNota;
			tracked.UserId = penjualan.UserId;

			tracked.Total = penjualan.Total;
			tracked.Dibayar = penjualan.Dibayar;
			tracked.Kembalian = penjualan.Kembalian;

			await _context.SaveChangesAsync();
		}

		private Penjualan CreatePenjualan(
			int userId,
			decimal dibayar,
			IEnumerable<KeranjangItems> keranjang)
		{
			var total = keranjang.Sum(x => x.SubTotal);

			return new Penjualan
			{
				UserId = userId,
				Tanggal = DateTime.Now,
				NomorNota = GenerateNota(),
				Total = total,
				Dibayar = dibayar,
				Kembalian = dibayar - total
			};
		}

		private async Task ProsesItemPenjualanAsync(
			int penjualanId,
			KeranjangItems item,
			Dictionary<int, ProdukSatuan> cache)
		{
			var satuanJual = await GetProdukSatuanJualAsync(item.ProdukSatuanId);

			if (!cache.TryGetValue(satuanJual.ProdukId, out var satuanDasar))
			{
				satuanDasar = await GetProdukSatuanDasarAsync(satuanJual.ProdukId);
				cache[satuanJual.ProdukId] = satuanDasar;
			}

			int qtyDasar = KonversiKeSatuanDasar(item.Jumlah, satuanJual);

			var detail = BuatDetailPenjualan(penjualanId, item, satuanJual.Id);

			await KurangiStokDanSimpanBatchAsync(
				detail,
				satuanDasar.Id,
				qtyDasar
			);
		}

		private async Task<ProdukSatuan> GetProdukSatuanJualAsync(int produkSatuanId)
		{
			return await _context.ProdukSatuans
				.FirstAsync(ps => ps.Id == produkSatuanId);
		}

		private async Task<ProdukSatuan> GetProdukSatuanDasarAsync(int produkId)
		{
			var satuanDasar = await _context.ProdukSatuans
				.Include(ps => ps.ProdukBatchs)
				.Where(ps => ps.ProdukId == produkId)
				.OrderBy(ps => ps.JumlahPerSatuan)
				.FirstOrDefaultAsync();

			if (satuanDasar == null)
				throw new Exception("Satuan dasar produk tidak ditemukan.");

			return satuanDasar;
		}

		private int KonversiKeSatuanDasar(int jumlahJual, ProdukSatuan satuanJual)
		{
			if (satuanJual.JumlahPerSatuan <= 0)
				throw new Exception("Konfigurasi satuan tidak valid.");

			return jumlahJual * satuanJual.JumlahPerSatuan;
		}

		private DetailPenjualan BuatDetailPenjualan(
			int penjualanId,
			KeranjangItems item,
			int produkSatuanJualId)
		{
			var detail = new DetailPenjualan
			{
				PenjualanId = penjualanId,
				ProdukSatuanId = produkSatuanJualId,
				Jumlah = item.Jumlah,
				Harga = item.HargaJual,
				Subtotal = item.SubTotal
			};

			_context.DetailPenjualans.Add(detail);

			return detail;
		}

		private async Task KurangiStokDanSimpanBatchAsync(
	DetailPenjualan detail,
	int produkSatuanDasarId,
	int qtyDasar)
		{
			var today = DateTime.Today;

			// 🔹 Ambil semua batch (termasuk expired)
			var allBatches = await _context.ProdukBatchs
				.Include(b => b.DetailPembelian)
					.ThenInclude(dp => dp!.ProdukSatuan)
				.Where(b =>
					b.ProdukSatuanId == produkSatuanDasarId &&
					b.Stok > 0)
				.ToListAsync();

			if (!allBatches.Any())
				throw new Exception("Stok produk tidak tersedia.");

			// 🔹 Hitung stok expired
			var expiredStock = allBatches
				.Where(b => b.TanggalKadarluasa != null && b.TanggalKadarluasa < today)
				.Sum(b => b.Stok);

			// 🔹 Ambil batch valid saja untuk penjualan
			var validBatches = allBatches
				.Where(b => b.TanggalKadarluasa == null || b.TanggalKadarluasa >= today)
				.OrderBy(b => b.TanggalKadarluasa == null)
				.ThenBy(b => b.TanggalKadarluasa)
				.ThenBy(b => b.TanggalMasuk)
				.ToList();

			if (!validBatches.Any())
				throw new Exception(
					$"Semua stok sudah expired. Stok expired tersisa: {expiredStock}"
				);

			int totalValidStock = validBatches.Sum(b => b.Stok);

			if (totalValidStock < qtyDasar)
				throw new Exception(
					$"Stok tidak mencukupi. Stok tersedia: {totalValidStock}"
				);

			// 🔹 proses pengurangan stok
			int sisa = qtyDasar;

			foreach (var batch in validBatches)
			{
				if (sisa <= 0)
					break;

				int ambil = Math.Min(batch.Stok, sisa);

				batch.Stok -= ambil;
				sisa -= ambil;

				var hargaBeliBox = batch.DetailPembelian!.HargaBeli;
				var isiPerBox = batch.DetailPembelian.ProdukSatuan.JumlahPerSatuan;
				var hargaBeliPerDasar = hargaBeliBox / isiPerBox;

				_context.DetailPenjualanBatches.Add(
					new DetailPenjualanBatch
					{
						DetailPenjualan = detail,
						ProdukBatchId = batch.Id,
						Jumlah = ambil,
						HargaBeli = hargaBeliPerDasar
					});
			}
		}

		private string GenerateNota()
		{
			return $"PJ-{DateTime.Now:yyyyMMddHHmmss}";
		}

		public async Task<StrukDto?> GetStrukAsync(int penjualanId)
		{
			var penjualan = await _context.Penjualans
				.IgnoreQueryFilters()
				.Include(p => p.DetailPenjualans)
					.ThenInclude(d => d.ProdukSatuan)
						.ThenInclude(ps => ps.Produk)
				.Include(p => p.DetailPenjualans)
					.ThenInclude(d => d.ProdukSatuan)
						.ThenInclude(ps => ps.Satuan)
				.FirstOrDefaultAsync(p => p.Id == penjualanId);

			if (penjualan == null)
				return null;

			var profile = await _context.Set<ProfileToko>()
				.AsNoTracking()
				.FirstOrDefaultAsync();

			var dto = new StrukDto
			{
				NamaToko = profile?.NamaToko ?? "",
				Alamat = profile?.Alamat ?? "",
				KataSambutan = profile?.KataSambutan,

				NomorNota = penjualan.NomorNota,
				Tanggal = penjualan.Tanggal,

				Total = penjualan.Total,
				Dibayar = penjualan.Dibayar,
				Kembalian = penjualan.Kembalian
			};

			foreach (var d in penjualan.DetailPenjualans)
			{
				dto.Items.Add(new StrukItemDto
				{
					NamaBarang = d.ProdukSatuan?.Produk?.NamaBarang ?? "Item",
					Satuan = d.ProdukSatuan?.Satuan?.NamaSatuan ?? "",
					Jumlah = d.Jumlah,
					Harga = d.Harga,
					Subtotal = d.Subtotal
				});
			}

			return dto;
		}
	}
}
