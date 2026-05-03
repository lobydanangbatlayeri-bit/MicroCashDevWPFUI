using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.CoreService;
using MicroCashDevWPFUI.Services.PembelianService;
using MicroCashDevWPFUI.Services.ProdukService;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public partial class PembelianPerbulanViewModel : PembelianBaseViewModel
	{
		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(TanggalText))]
		private DateTime bulan = DateTime.Now;
		[ObservableProperty]
		private string nomorFaktur = string.Empty;
		[ObservableProperty]
		private ObservableCollection<string> nomorFakturList = new();

		public string TanggalText =>
			Bulan.ToString("MMMM yyyy", new CultureInfo("id-ID"));

		public PembelianPerbulanViewModel(
			IPembelianService pembelianService,
			IDetailPembelianService detailService,
			IProdukBatchService produkBatchService,
			IPembelianExportService exportService,
			IDialogService dialogService,
			ISupplierService supplierService,
			ISatuanService satuanService)
			: base(pembelianService, detailService, produkBatchService, exportService, dialogService, supplierService, satuanService)
		{
			_ = LoadDataAsync();
		}

		partial void OnBulanChanged(DateTime oldValue, DateTime newValue)
		{
			_ = LoadDataAsync();
		}

		partial void OnNomorFakturChanged(string? oldValue, string newValue)
		{
			CurrentPage = 1;
			_ = LoadDataAsync();
		}

		protected override IEnumerable<Pembelian> ApplyFilter(IEnumerable<Pembelian> source)
		{
			NomorFakturList = new ObservableCollection<string>(
				source
					.Where(p => !string.IsNullOrWhiteSpace(p.NomorFaktur))
					.Select(p => p.NomorFaktur!)
					.Distinct()
			);

			if (string.IsNullOrWhiteSpace(NomorFaktur))
				return source;

			return source.Where(p =>
				(p.NomorFaktur ?? "")
				.Contains(NomorFaktur, StringComparison.OrdinalIgnoreCase));
		}

		protected override (DateTime start, DateTime end) GetRange()
		{
			var start = new DateTime(Bulan.Year, Bulan.Month, 1);
			return (start, start.AddMonths(1));
		}

		[RelayCommand]
		private void TanggalBack()
		{
			Bulan = Bulan.AddMonths(-1);
		}

		[RelayCommand]
		private void TanggalNext()
		{
			Bulan = Bulan.AddMonths(1);
		}
	}

}
