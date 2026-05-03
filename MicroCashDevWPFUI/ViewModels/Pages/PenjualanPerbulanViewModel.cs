using ClosedXML.Excel;
using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.CoreService;
using MicroCashDevWPFUI.Services.PenjualanService;
using MicroCashDevWPFUI.Services.ProdukService;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public partial class PenjualanPerbulanViewModel : PenjualanBaseViewModel
	{
		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(TanggalText))]
		private DateTime tanggal = DateTime.Now;

		[ObservableProperty]
		private string nomorNota = string.Empty;

		[ObservableProperty]
		private ObservableCollection<string> nomorNotaList = new();

		public string TanggalText =>
			Tanggal.ToString("MMMM yyyy", new CultureInfo("id-ID"));

		public PenjualanPerbulanViewModel(
			IPenjualanService penjualanService,
			IDetailPenjualanService detailService,
			IProdukBatchService produkBatchService,
			IPenjualanExportService exportService,
			IDialogService dialogService,
			ISatuanService satuanService)
			: base(
				  penjualanService,
				  detailService,
				  produkBatchService,
				  exportService,
				  dialogService,
				  satuanService)
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

		protected override IEnumerable<Penjualan> ApplyFilter(IEnumerable<Penjualan> source)
		{
			NomorNotaList = new ObservableCollection<string>(
				source
					.Where(p => !string.IsNullOrWhiteSpace(p.NomorNota))
					.Select(p => p.NomorNota!)
					.Distinct()
			);

			if (string.IsNullOrWhiteSpace(NomorNota))
				return source;

			return source.Where(p =>
				(p.NomorNota ?? "")
				.Contains(NomorNota, StringComparison.OrdinalIgnoreCase));
		}

		// ================= RANGE =================

		protected override (DateTime start, DateTime end) GetRange()
		{
			var start = new DateTime(Tanggal.Year, Tanggal.Month, 1);
			var end = start.AddMonths(1);
			return (start, end);
		}

		// ================= NAVIGASI TANGGAL =================

		[RelayCommand]
		private void TanggalBack()
		{
			Tanggal = Tanggal.AddMonths(-1);
		}

		[RelayCommand]
		private void TanggalNext()
		{
			Tanggal = Tanggal.AddMonths(1);
		}
	}
}
