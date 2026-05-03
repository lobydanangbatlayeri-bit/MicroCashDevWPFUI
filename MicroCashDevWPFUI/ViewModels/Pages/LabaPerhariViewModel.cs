using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Services.CoreService;
using MicroCashDevWPFUI.Services.KeuanganService;
using MicroCashDevWPFUI.Services.PenjualanService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public partial class LabaPerhariViewModel : LabaBaseViewModel
	{
		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(TanggalText))]
		private DateTime tanggal = DateTime.Now;

		[ObservableProperty]
		private string nomorNota = string.Empty;

		[ObservableProperty]
		private ObservableCollection<string> nomorNotaList = new();

		public string TanggalText =>
			Tanggal.ToString("dddd, dd/MM/yyyy", new CultureInfo("id-ID"));

		public LabaPerhariViewModel(
			IPenjualanService penjualanService,
			IDialogService dialogService,
			ILabaExportService labaExportService)
			: base(penjualanService, dialogService, labaExportService)
		{
			_ = LoadDataAsync();
		}

		// ================= TRIGGER =================

		partial void OnTanggalChanged(DateTime oldValue, DateTime newValue)
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

		// ================= RANGE =================

		protected override (DateTime start, DateTime end) GetRange()
		{
			var start = Tanggal.Date;
			return (start, start.AddDays(1));
		}

		// ================= NAVIGASI TANGGAL =================

		[RelayCommand]
		private void TanggalBack()
		{
			Tanggal = Tanggal.AddDays(-1);
		}

		[RelayCommand]
		private void TanggalNext()
		{
			Tanggal = Tanggal.AddDays(1);
		}
	}
}
