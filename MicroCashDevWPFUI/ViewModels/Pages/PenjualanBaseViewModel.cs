using DocumentFormat.OpenXml.Wordprocessing;
using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.CoreService;
using MicroCashDevWPFUI.Services.PenjualanService;
using MicroCashDevWPFUI.Services.ProdukService;
using System.Collections.ObjectModel;
using System.Globalization;
using static MicroCashDevWPFUI.DTOs.PenjualanItem;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public abstract partial class PenjualanBaseViewModel : ObservableObject
	{
		protected readonly IPenjualanService _penjualanService;
		protected readonly IDetailPenjualanService _detailService;
		protected readonly IProdukBatchService _produkBatchService;
		protected readonly IPenjualanExportService _exportService;
		protected readonly IDialogService _dialogService;
		protected readonly ISatuanService _satuanService;

		protected List<PenjualanItem> _allItems = new();

		[ObservableProperty]
		protected ObservableCollection<PenjualanItem> itemsPenjualan = new();

		[ObservableProperty]
		private int currentPage = 1;

		[ObservableProperty]
		private int pageSize = 100;

		[ObservableProperty]
		private ObservableCollection<Satuan> itemsSatuan = new();

		public int TotalPages =>
			(int)Math.Ceiling((double)_allItems.Count / PageSize);

		public string PageInfo =>
			$"Halaman {CurrentPage} dari {TotalPages}";

		public decimal TotalPenjualan =>
			ItemsPenjualan.Sum(x => x.Total);

		public string TotalPenjualanText =>
			TotalPenjualan.ToString("C", new CultureInfo("id-ID"));

		protected PenjualanBaseViewModel(
			IPenjualanService penjualanService,
			IDetailPenjualanService detailService,
			IProdukBatchService produkBatchService,
			IPenjualanExportService exportService,
			IDialogService dialogService,
			ISatuanService satuanService)
		{
			_penjualanService = penjualanService;
			_detailService = detailService;
			_produkBatchService = produkBatchService;
			_exportService = exportService;
			_dialogService = dialogService;
			_satuanService = satuanService;

			ItemsPenjualan.CollectionChanged += (_, __) =>
			{
				OnPropertyChanged(nameof(TotalPenjualan));
				OnPropertyChanged(nameof(TotalPenjualanText));
			};
		}

		protected abstract (DateTime start, DateTime end) GetRange();

		protected virtual IEnumerable<Penjualan> ApplyFilter(IEnumerable<Penjualan> source)
		{
			return source;
		}

		protected void ApplyPaging()
		{
			ItemsPenjualan.Clear();

			var pageData = _allItems
				.Skip((CurrentPage - 1) * PageSize)
				.Take(PageSize);

			foreach (var item in pageData)
				ItemsPenjualan.Add(item);

			OnPropertyChanged(nameof(PageInfo));
		}

		private bool CanNextPage() => CurrentPage < TotalPages;
		private bool CanPrevPage() => CurrentPage > 1;

		partial void OnCurrentPageChanged(int value)
		{
			OnPropertyChanged(nameof(PageInfo));
			PrevPageCommand.NotifyCanExecuteChanged();
			NextPageCommand.NotifyCanExecuteChanged();
		}

		// ================= LOAD DATA =================

		[RelayCommand]
		protected async Task LoadDataAsync()
		{
			_allItems.Clear();
			ItemsPenjualan.Clear();

			ItemsSatuan = new ObservableCollection<Satuan>(
				await _satuanService.GetAllAsync()
			);

			var (start, end) = GetRange();
			var penjualanList = await _penjualanService.GetRiwayatAsync(start, end);
			penjualanList = ApplyFilter(penjualanList).ToList();

			foreach (var penjualan in penjualanList)
			{
				var detailDTOs = new ObservableCollection<DetailPenjualanItem>(
					penjualan.DetailPenjualans.Select(detail =>
						new DetailPenjualanItem(_detailService, detail.Id)
						{
							Id = detail.Id,
							Produk = detail.ProdukSatuan?.Produk?.NamaBarang ?? "",
							Satuan = detail.ProdukSatuan?.Satuan,
							Jumlah = detail.Jumlah,
							Harga = detail.Harga,
							Subtotal = detail.Subtotal
						})
				);

				_allItems.Add(new PenjualanItem(_penjualanService, penjualan.Id)
				{
					Id = penjualan.Id,
					Nomor = penjualan.Id,
					Tanggal = penjualan.Tanggal,
					NomorNota = penjualan.NomorNota ?? "",
					Item = penjualan.DetailPenjualans.Count,
					Total = penjualan.Total,
					Dibayar = penjualan.Dibayar,
					Kembalian = penjualan.Kembalian,
					UserId = penjualan.UserId,
					Kasir = penjualan.User?.Username ?? "",
					DetailPenjualans = detailDTOs
				});
			}

			CurrentPage = 1;
			ApplyPaging();
		}

		// ================= PAGING =================

		[RelayCommand(CanExecute = nameof(CanPrevPage))]
		private void PrevPage()
		{
			CurrentPage--;
			ApplyPaging();
		}

		[RelayCommand(CanExecute = nameof(CanNextPage))]
		private void NextPage()
		{
			CurrentPage++;
			ApplyPaging();
		}

		// ================= EXPORT =================

		[RelayCommand]
		protected async Task ExportAsync()
		{
			if (!ItemsPenjualan.Any())
			{
				await _dialogService.ShowMessage("Tidak ada data untuk diexport.");
				return;
			}

			var (start, end) = GetRange();
			await _exportService.ExportAsync(_allItems, start, end);

			await _dialogService.ShowMessage("Export berhasil.");
		}
	}
}
