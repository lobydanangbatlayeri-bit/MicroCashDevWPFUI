using ClosedXML.Excel;
using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.CoreService;
using MicroCashDevWPFUI.Services.PembelianService;
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
	public partial class NotaSudahBayarViewModel : NotaBaseViewModel
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

		public NotaSudahBayarViewModel(
			INotaService notaService,
			INotaExportService exportService,
			IDialogService dialogService,
			ISupplierService supplierService)
			: base(notaService, dialogService, supplierService, exportService)
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

		protected override IEnumerable<Nota> ApplyFilter(IEnumerable<Nota> source)
		{
			// Hanya yang sudah dibayar
			var filtered = source.Where(n => n.StatusPembayaran);

			// Update AutoSuggest list
			NomorFakturList = new ObservableCollection<string>(
				filtered
					.Where(n => !string.IsNullOrWhiteSpace(n.Pembelian.NomorFaktur))
					.Select(n => n.Pembelian.NomorFaktur!)
					.Distinct()
			);

			// Filter berdasarkan nomor faktur jika diisi
			if (!string.IsNullOrWhiteSpace(NomorFaktur))
			{
				filtered = filtered.Where(n =>
					(n.Pembelian.NomorFaktur ?? "")
					.Contains(NomorFaktur, StringComparison.OrdinalIgnoreCase));
			}

			return filtered;
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
