using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.PenjualanService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.DTOs
{
	public partial class LabaItem : ObservableObject
	{
		private readonly IPenjualanService? _penjualanService;

		public LabaItem() { }

		public LabaItem(IPenjualanService penjualanService, int penjualanId)
		{
			_penjualanService = penjualanService;
			Id = penjualanId;
			Debug.WriteLine($"[DEBUG] LabaItem created: Id={Id}");
		}

		public int Id { get; init; }
		public int Nomor { get; set; }

		public ObservableCollection<LabaDetailItem> DetailLabas { get; set; } = new();

		[ObservableProperty] private DateTime tanggal;
		[ObservableProperty] private string nomorNota = "";
		[ObservableProperty] private int item;
		[ObservableProperty] private decimal totalPenjualan;
		[ObservableProperty] private decimal totalHpp;

		public decimal Laba => TotalPenjualan - TotalHpp;

		public string TotalPenjualanText =>
			TotalPenjualan.ToString("C", new CultureInfo("id-ID"));

		public string TotalHppText =>
			TotalHpp.ToString("C", new CultureInfo("id-ID"));

		public string LabaText =>
			Laba.ToString("C", new CultureInfo("id-ID"));

		partial void OnTotalPenjualanChanged(decimal value)
		{
			OnPropertyChanged(nameof(Laba));
			OnPropertyChanged(nameof(LabaText));
		}

		partial void OnTotalHppChanged(decimal value)
		{
			OnPropertyChanged(nameof(Laba));
			OnPropertyChanged(nameof(LabaText));
		}

		// ================= DETAIL =================

		public partial class LabaDetailItem : ObservableObject
		{
			public LabaDetailItem() { }

			public int Id { get; init; }

			public string Produk { get; set; } = "";
			public Satuan? Satuan { get; set; }

			[ObservableProperty] private int jumlah;
			[ObservableProperty] private decimal hargaJual;
			[ObservableProperty] private decimal subtotal;
			[ObservableProperty] private decimal hpp;

			public decimal Laba => Subtotal - Hpp;

			public string SubtotalText =>
				Subtotal.ToString("C", new CultureInfo("id-ID"));

			public string HppText =>
				Hpp.ToString("C", new CultureInfo("id-ID"));

			public string LabaText =>
				Laba.ToString("C", new CultureInfo("id-ID"));

			partial void OnSubtotalChanged(decimal value)
			{
				OnPropertyChanged(nameof(Laba));
				OnPropertyChanged(nameof(LabaText));
			}

			partial void OnHppChanged(decimal value)
			{
				OnPropertyChanged(nameof(Laba));
				OnPropertyChanged(nameof(LabaText));
			}
		}
	}

}
