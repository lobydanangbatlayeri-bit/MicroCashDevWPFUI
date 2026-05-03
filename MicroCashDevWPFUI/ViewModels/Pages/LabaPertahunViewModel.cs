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
	public partial class LabaPertahunViewModel : LabaBaseViewModel
	{
		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(TahunText))]
		private int tahun = DateTime.Now.Year;

		[ObservableProperty]
		private string nomorNota = string.Empty;

		[ObservableProperty]
		private ObservableCollection<string> nomorNotaList = new();

		public string TahunText => Tahun.ToString();

		public LabaPertahunViewModel(
			IPenjualanService penjualanService,
			IDialogService dialogService,
			ILabaExportService labaExportService)
			: base(penjualanService, dialogService, labaExportService)
		{
			_ = LoadDataAsync();
		}

		// ================= TRIGGER =================

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

		// ================= RANGE TAHUN =================

		protected override (DateTime start, DateTime end) GetRange()
		{
			var start = new DateTime(Tahun, 1, 1);
			var end = start.AddYears(1); // awal tahun berikutnya
			return (start, end);
		}

		// ================= NAVIGASI TAHUN =================

		[RelayCommand]
		private void TahunBack()
		{
			Tahun--;
		}

		[RelayCommand]
		private void TahunNext()
		{
			Tahun++;
		}
	}
}
