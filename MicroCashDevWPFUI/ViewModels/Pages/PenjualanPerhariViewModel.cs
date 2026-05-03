using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.CoreService;
using MicroCashDevWPFUI.Services.PenjualanService;
using MicroCashDevWPFUI.Services.ProdukService;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public partial class PenjualanPerhariViewModel : PenjualanBaseViewModel
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

		public PenjualanPerhariViewModel(
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
