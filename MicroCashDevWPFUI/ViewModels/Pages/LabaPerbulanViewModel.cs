using ClosedXML.Excel;
using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Services.CoreService;
using MicroCashDevWPFUI.Services.KeuanganService;
using MicroCashDevWPFUI.Services.PenjualanService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public partial class LabaPerbulanViewModel : LabaBaseViewModel
	{
		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(BulanText))]
		private int bulan = DateTime.Now.Month;

		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(BulanText))]
		private int tahun = DateTime.Now.Year;

		[ObservableProperty]
		private string nomorNota = string.Empty;

		[ObservableProperty]
		private ObservableCollection<string> nomorNotaList = new();

		public string BulanText =>
			new DateTime(Tahun, Bulan, 1).ToString("MMMM yyyy", new CultureInfo("id-ID"));

		public LabaPerbulanViewModel(
			IPenjualanService penjualanService,
			IDialogService dialogService,
			ILabaExportService labaExportService)
			: base(penjualanService, dialogService, labaExportService)
		{
			_ = LoadDataAsync();
		}

		// ================= TRIGGER =================

		partial void OnBulanChanged(int oldValue, int newValue)
		{
			_ = LoadDataAsync();
		}

		partial void OnTahunChanged(int oldValue, int newValue)
		{
			_ = LoadDataAsync();
		}

		partial void OnNomorNotaChanged(string? oldValue, string newValue)
		{
			CurrentPage = 1;
			_ = LoadDataAsync();
		}

		// ================= FILTER =================

		protected override IEnumerable<LabaItem> ApplyFilter(IEnumerable<LabaItem> source)
		{
			NomorNotaList = new ObservableCollection<string>(
				source
					.Where(p => !string.IsNullOrWhiteSpace(p.NomorNota))
					.Select(p => p.NomorNota)
					.Distinct()
			);

			if (string.IsNullOrWhiteSpace(NomorNota))
				return source;

			return source.Where(p =>
				p.NomorNota.Contains(NomorNota, StringComparison.OrdinalIgnoreCase));
		}

		// ================= RANGE BULAN =================

		protected override (DateTime start, DateTime end) GetRange()
		{
			var start = new DateTime(Tahun, Bulan, 1);
			var end = start.AddMonths(1); // awal bulan berikutnya
			return (start, end);
		}

		// ================= NAVIGASI BULAN =================

		[RelayCommand]
		private void BulanBack()
		{
			var prevMonth = new DateTime(Tahun, Bulan, 1).AddMonths(-1);
			Bulan = prevMonth.Month;
			Tahun = prevMonth.Year;
		}

		[RelayCommand]
		private void BulanNext()
		{
			var nextMonth = new DateTime(Tahun, Bulan, 1).AddMonths(1);
			Bulan = nextMonth.Month;
			Tahun = nextMonth.Year;
		}
	}
}
