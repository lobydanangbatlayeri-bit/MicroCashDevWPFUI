using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.CoreService;
using MicroCashDevWPFUI.Services.PembelianService;
using MicroCashDevWPFUI.Services.ProdukService;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public partial class PembelianPerhariViewModel : PembelianBaseViewModel
	{
		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(TanggalText))]
		private DateTime tanggal = DateTime.Now;
		[ObservableProperty]
		private string nomorFaktur = string.Empty;
		[ObservableProperty]
		private ObservableCollection<string> nomorFakturList = new();

		public string TanggalText =>
			Tanggal.ToString("dddd, dd/MM/yyyy", new CultureInfo("id-ID"));

		public PembelianPerhariViewModel(
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

		partial void OnTanggalChanged(DateTime oldValue, DateTime newValue)
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
			var start = Tanggal.Date;
			return (start, start.AddDays(1));
		}

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
