using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.CoreService;
using MicroCashDevWPFUI.Services.PenjualanService;
using MicroCashDevWPFUI.Services.ProdukService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public partial class PenjualanPertahunViewModel : PenjualanBaseViewModel
	{
		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(TanggalText))]
		private DateTime tanggal = DateTime.Now;

		[ObservableProperty]
		private string nomorNota = string.Empty;

		[ObservableProperty]
		private ObservableCollection<string> nomorNotaList = new();

		public string TanggalText =>
			Tanggal.ToString("yyyy", new CultureInfo("id-ID"));

		public PenjualanPertahunViewModel(
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
			var start = new DateTime(Tanggal.Year, 1, 1);
			var end = start.AddYears(1);
			return (start, end);
		}

		// ================= NAVIGASI TANGGAL =================

		[RelayCommand]
		private void TanggalBack()
		{
			Tanggal = Tanggal.AddYears(-1);
		}

		[RelayCommand]
		private void TanggalNext()
		{
			Tanggal = Tanggal.AddYears(1);
		}
	}
}
