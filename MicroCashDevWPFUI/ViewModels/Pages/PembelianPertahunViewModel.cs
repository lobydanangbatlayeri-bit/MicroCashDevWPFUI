using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.CoreService;
using MicroCashDevWPFUI.Services.PembelianService;
using MicroCashDevWPFUI.Services.ProdukService;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public partial class PembelianPertahunViewModel : PembelianBaseViewModel
	{
		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(TanggalText))]
		private DateTime tahun = DateTime.Now;

		[ObservableProperty]
		private string nomorFaktur = string.Empty;

		[ObservableProperty]
		private ObservableCollection<string> nomorFakturList = new();

		public string TanggalText =>
			Tahun.ToString("yyyy", new CultureInfo("id-ID"));

		public PembelianPertahunViewModel(
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

		partial void OnTahunChanged(DateTime oldValue, DateTime newValue)
		{
			CurrentPage = 1;
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
			var start = new DateTime(Tahun.Year, 1, 1);
			return (start, start.AddYears(1));
		}

		[RelayCommand]
		private void TanggalBack()
		{
			Tahun = Tahun.AddYears(-1);
		}

		[RelayCommand]
		private void TanggalNext()
		{
			Tahun = Tahun.AddYears(1);
		}
	}
}