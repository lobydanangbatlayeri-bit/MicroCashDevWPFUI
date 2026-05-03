using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using MicroCashDevWPFUI.DTOs.Interfaces;
using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.PenjualanService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.DTOs
{
	// === PenjualanItem ===
	public partial class PenjualanItem : ObservableObject, IAutoSaveItem
	{
		private readonly IPenjualanService? _penjualanService;

		public PenjualanItem() { }

		public PenjualanItem(IPenjualanService penjualanService, int penjualanId)
		{
			_penjualanService = penjualanService;
			Id = penjualanId;
			Debug.WriteLine($"[DEBUG] PenjualanItem created: Id={Id}");
		}

		public int Id { get; init; }
		public int Nomor { get; set; }

		public ObservableCollection<DetailPenjualanItem> DetailPenjualans { get; set; } = new();

		[ObservableProperty] private DateTime tanggal;
		[ObservableProperty] private string nomorNota = "";
		[ObservableProperty] private int item;
		[ObservableProperty] private decimal total;
		[ObservableProperty] private int userId;
		[ObservableProperty] private string kasir = string.Empty;
		[ObservableProperty] private decimal dibayar;
		[ObservableProperty] private decimal kembalian;

		public async Task SaveAsync()
		{
			if (_penjualanService == null)
			{
				Debug.WriteLine("[DEBUG] PenjualanItem.SaveAsync skipped: _penjualanService is null");
				return;
			}

			var entity = await _penjualanService.GetByIdAsync(Id);

			if (entity != null)
			{
				Debug.WriteLine($"[DEBUG] Saving PenjualanItem Id={Id}: Total={Total}, Dibayar={Dibayar}");

				entity.Tanggal = Tanggal;
				entity.NomorNota = NomorNota;
				entity.UserId = UserId;

				entity.Total = Total;
				entity.Dibayar = Dibayar;
				entity.Kembalian = Kembalian;

				await _penjualanService.UpdateAsync(entity);
			}
			else
			{
				Debug.WriteLine($"[DEBUG] PenjualanItem.SaveAsync: entity Id={Id} not found!");
			}
		}

		// === DetailPenjualanItem ===
		public partial class DetailPenjualanItem : ObservableObject, IAutoSaveItem
		{
			private readonly IDetailPenjualanService _detailService;

			public DetailPenjualanItem(IDetailPenjualanService detailService, int id)
			{
				_detailService = detailService;
				Id = id;
				Debug.WriteLine($"[DEBUG] DetailPenjualanItem created: Id={Id}");
			}

			public int Id { get; init; }

			public string Produk { get; set; } = "";
			public Satuan? Satuan { get; set; }

			public ObservableCollection<ProdukBatchItem> ProdukBatchs { get; set; } = new();

			[ObservableProperty] private int jumlah;
			[ObservableProperty] private decimal harga;
			[ObservableProperty] private decimal subtotal;

			public async Task SaveAsync()
			{
				var entity = await _detailService.GetByIdAsync(Id);
				if (entity != null)
				{
					Debug.WriteLine($"[DEBUG] Saving DetailPenjualanItem Id={Id}: Jumlah={Jumlah}, HargaJual={Harga} Subtotal={Subtotal}");

					entity.Jumlah = Jumlah;
					entity.Harga = Harga;
					entity.Subtotal = Subtotal;

					await _detailService.UpdateAsync(entity);
				}
				else
				{
					Debug.WriteLine($"[DEBUG] DetailPenjualanItem.SaveAsync: entity Id={Id} not found!");
				}
			}
		}
	}
}
