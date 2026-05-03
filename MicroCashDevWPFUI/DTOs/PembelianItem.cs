using CommunityToolkit.Mvvm.ComponentModel;
using MicroCashDevWPFUI.DTOs.Interfaces;
using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.PembelianService;
using MicroCashDevWPFUI.Services.ProdukService;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.DTOs
{
	// === PembelianItem ===
	public partial class PembelianItem : ObservableObject, IAutoSaveItem
	{
		private readonly IPembelianService? _pembelianService;

		public PembelianItem() { }

		public PembelianItem(IPembelianService pembelianService, int pembelianId)
		{
			_pembelianService = pembelianService;
			Id = pembelianId;
			Debug.WriteLine($"[DEBUG] PembelianItem created: Id={Id}");
		}

		public int Id { get; init; }
		public int Nomor { get; set; }
		public ObservableCollection<DetailPembelianItem> DetailPembelians { get; set; } = new();

		[ObservableProperty] private DateTime tanggal;
		[ObservableProperty] private string nomorFaktur = "";
		[ObservableProperty] private Supplier? supplier;
		[ObservableProperty] private int item;
		[ObservableProperty] private decimal total;

		public async Task SaveAsync()
		{
			if (_pembelianService == null)
			{
				Debug.WriteLine("[DEBUG] PembelianItem.SaveAsync skipped: _pembelianService is null");
				return;
			}

			var entity = await _pembelianService.GetByIdAsync(Id);
			if (entity != null)
			{
				Debug.WriteLine($"[DEBUG] Saving PembelianItem Id={Id}: Total={Total}, Supplier={Supplier?.NamaSupplier}, NomorFaktur={NomorFaktur}");

				entity.Tanggal = Tanggal;
				entity.NomorFaktur = NomorFaktur;
				if (Supplier != null)
					entity.Supplier = Supplier;
				else
					Debug.WriteLine($"[DEBUG] Supplier is null for PembelianItem Id={Id}");
				entity.Total = Total;

				await _pembelianService.UpdateAsync(entity);
			}
			else
			{
				Debug.WriteLine($"[DEBUG] PembelianItem.SaveAsync: entity Id={Id} not found!");
			}
		}
	}

	// === DetailPembelianItem ===
	public partial class DetailPembelianItem : ObservableObject, IAutoSaveItem
	{
		private readonly IDetailPembelianService _detailService;

		public DetailPembelianItem(IDetailPembelianService detailService, int id)
		{
			_detailService = detailService;
			Id = id;
			Debug.WriteLine($"[DEBUG] DetailPembelianItem created: Id={Id}");
		}

		public int Id { get; init; }
		public string Produk { get; set; } = "";
		public Satuan? Satuan { get; set; }
		public ObservableCollection<ProdukBatchItem> ProdukBatchs { get; set; } = new();

		[ObservableProperty] private int jumlah;
		[ObservableProperty] private decimal hargaBeli;
		[ObservableProperty] private decimal subtotal;

		public async Task SaveAsync()
		{
			var entity = await _detailService.GetByIdAsync(Id);
			if (entity != null)
			{
				Debug.WriteLine($"[DEBUG] Saving DetailPembelianItem Id={Id}: Jumlah={Jumlah}, HargaBeli={HargaBeli}, Subtotal={Subtotal}");
				entity.Jumlah = Jumlah;
				entity.HargaBeli = HargaBeli;
				entity.Subtotal = Subtotal;
				await _detailService.UpdateAsync(entity);
			}
			else
			{
				Debug.WriteLine($"[DEBUG] DetailPembelianItem.SaveAsync: entity Id={Id} not found!");
			}
		}
	}

	// === ProdukBatchItem ===
	public partial class ProdukBatchItem : ObservableObject, IAutoSaveItem
	{
		private readonly IProdukBatchService? _batchService;

		public ProdukBatchItem() { }

		public ProdukBatchItem(IProdukBatchService batchService, int batchId)
		{
			_batchService = batchService;
			Id = batchId;
			Debug.WriteLine($"[DEBUG] ProdukBatchItem created: Id={Id}");
		}

		public int Id { get; init; }

		[ObservableProperty] private string batchNumber = "";
		[ObservableProperty] private DateTime tanggalKadarluasa;
		[ObservableProperty] private int stok;

		public async Task SaveAsync()
		{
			if (_batchService == null)
			{
				Debug.WriteLine("[DEBUG] ProdukBatchItem.SaveAsync skipped: _batchService is null");
				return;
			}

			var entity = await _batchService.GetByIdAsync(Id);
			if (entity != null)
			{
				Debug.WriteLine($"[DEBUG] Saving ProdukBatchItem Id={Id}: BatchNumber={BatchNumber}, Stok={Stok}, Kadarluasa={TanggalKadarluasa}");
				entity.BatchNumber = BatchNumber;
				entity.TanggalKadarluasa = TanggalKadarluasa;
				entity.Stok = Stok;

				await _batchService.UpdateAsync(entity);
			}
			else
			{
				Debug.WriteLine($"[DEBUG] ProdukBatchItem.SaveAsync: entity Id={Id} not found!");
			}
		}
	}
}
