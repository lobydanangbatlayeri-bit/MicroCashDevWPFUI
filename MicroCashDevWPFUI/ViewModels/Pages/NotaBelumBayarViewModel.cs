using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.CoreService;
using MicroCashDevWPFUI.Services.PembelianService;
using MicroCashDevWPFUI.Services.PenjualanService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public partial class NotaBelumBayarViewModel : NotaBaseViewModel
	{
		[ObservableProperty]
		private string nomorFaktur = string.Empty;

		[ObservableProperty]
		private ObservableCollection<string> nomorFakturList = new();

		public NotaBelumBayarViewModel(
			INotaService notaService,
			INotaExportService exportService,
			IDialogService dialogService,
			ISupplierService supplierService)
			: base(notaService, dialogService, supplierService, exportService)
		{
			_ = LoadDataAsync();
		}

		partial void OnNomorFakturChanged(string? oldValue, string newValue)
		{
			CurrentPage = 1;
			_ = LoadDataAsync();
		}

		protected override (DateTime start, DateTime end) GetRange()
		{
			// Ambil semua tanggal, karena kita ingin semua nota belum bayar
			return (DateTime.MinValue, DateTime.MaxValue);
		}

		protected override IEnumerable<Nota> ApplyFilter(IEnumerable<Nota> source)
		{
			// Hanya yang belum dibayar
			var filtered = source.Where(n => !n.StatusPembayaran);

			// Update NomorFakturList untuk AutoSuggestBox
			NomorFakturList = new ObservableCollection<string>(
				filtered
					.Where(n => !string.IsNullOrWhiteSpace(n.Pembelian.NomorFaktur))
					.Select(n => n.Pembelian.NomorFaktur!)
					.Distinct()
			);

			// Jika NomorFaktur diisi, filter lagi
			if (!string.IsNullOrWhiteSpace(NomorFaktur))
			{
				filtered = filtered.Where(n =>
					(n.Pembelian.NomorFaktur ?? "")
					.Contains(NomorFaktur, StringComparison.OrdinalIgnoreCase)
				);
			}

			return filtered;
		}
	}
}