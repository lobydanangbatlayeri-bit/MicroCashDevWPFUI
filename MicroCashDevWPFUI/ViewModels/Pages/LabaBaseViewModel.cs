using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Wordprocessing;
using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.CoreService;
using MicroCashDevWPFUI.Services.KeuanganService;
using MicroCashDevWPFUI.Services.PenjualanService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static MicroCashDevWPFUI.DTOs.LabaItem;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public abstract partial class LabaBaseViewModel : ObservableObject
	{
		protected readonly IPenjualanService _penjualanService;
		protected readonly IDialogService _dialogService;
		protected readonly ILabaExportService _exportService;

		protected List<LabaItem> _allItems = new();
		protected List<LabaItem> _filteredItems = new();

		[ObservableProperty]
		private ObservableCollection<LabaItem> itemsLaba = new();

		[ObservableProperty]
		private int currentPage = 1;

		[ObservableProperty]
		private int pageSize = 100;

		public int TotalPages =>
			(int)Math.Ceiling((double)_filteredItems.Count / PageSize);

		public string PageInfo =>
			$"Halaman {CurrentPage} dari {TotalPages}";

		// ================= TOTAL (FULL FILTERED DATA) =================

		public decimal TotalPenjualan =>
			_filteredItems.Sum(x => x.TotalPenjualan);

		public decimal TotalBiaya =>
			_filteredItems.Sum(x => x.TotalHpp);

		public decimal LabaBersih =>
			TotalPenjualan - TotalBiaya;

		public string TotalPenjualanText =>
			FormatCurrency(TotalPenjualan);

		public string TotalBiayaText =>
			FormatCurrency(TotalBiaya);

		public string LabaBersihText =>
			FormatCurrency(LabaBersih);

		protected LabaBaseViewModel(
			IPenjualanService penjualanService,
			IDialogService dialogService,
			ILabaExportService exportService)
		{
			_penjualanService = penjualanService;
			_dialogService = dialogService;
			_exportService = exportService;
		}

		protected abstract (DateTime start, DateTime end) GetRange();

		protected virtual IEnumerable<LabaItem> ApplyFilter(IEnumerable<LabaItem> source)
			=> source;

		// ================= LOAD =================

		[RelayCommand]
		protected async Task LoadDataAsync()
		{
			_allItems.Clear();
			ItemsLaba.Clear();

			var (start, end) = GetRange();
			var penjualanList = await _penjualanService.GetRiwayatAsync(start, end);

			foreach (var penjualan in penjualanList)
			{
				decimal totalHpp = 0;
				var detailList = new ObservableCollection<LabaDetailItem>();

				foreach (var detail in penjualan.DetailPenjualans)
				{
					var batches = detail.DetailPenjualanBatches ?? Enumerable.Empty<DetailPenjualanBatch>();

					Debug.WriteLine(
						$"PenjualanId={penjualan.Id} | DetailId={detail.Id} | BatchCount={batches.Count()}"
					);

					var hppDetail = batches.Sum(b => b.Jumlah * b.HargaBeli);

					totalHpp += hppDetail;

					detailList.Add(new LabaDetailItem
					{
						Produk = detail.ProdukSatuan?.Produk?.NamaBarang ?? "",
						Jumlah = detail.Jumlah,
						HargaJual = detail.Harga,
						Subtotal = detail.Subtotal,
						Hpp = hppDetail
					});
				}

				_allItems.Add(new LabaItem
				{
					Id = penjualan.Id,
					Tanggal = penjualan.Tanggal,
					NomorNota = penjualan.NomorNota ?? "",
					TotalPenjualan = penjualan.Total,
					TotalHpp = totalHpp,
					DetailLabas = detailList
				});
			}
			ApplyFilterAndPaging();
		}

		// ================= FILTER + PAGING CORE =================

		protected void ApplyFilterAndPaging()
		{
			_filteredItems = ApplyFilter(_allItems).ToList();

			CurrentPage = 1;
			ApplyPaging();
			RaiseTotalsChanged();
		}

		protected void ApplyPaging()
		{
			ItemsLaba.Clear();

			var pageData = _filteredItems
				.Skip((CurrentPage - 1) * PageSize)
				.Take(PageSize);

			foreach (var item in pageData)
				ItemsLaba.Add(item);

			OnPropertyChanged(nameof(PageInfo));
			OnPropertyChanged(nameof(TotalPages));
		}

		partial void OnCurrentPageChanged(int value)
		{
			ApplyPaging();
			PrevPageCommand.NotifyCanExecuteChanged();
			NextPageCommand.NotifyCanExecuteChanged();
		}

		private bool CanNextPage() => CurrentPage < TotalPages;
		private bool CanPrevPage() => CurrentPage > 1;

		[RelayCommand(CanExecute = nameof(CanPrevPage))]
		private void PrevPage() => CurrentPage--;

		[RelayCommand(CanExecute = nameof(CanNextPage))]
		private void NextPage() => CurrentPage++;

		// ================= EXPORT =================
		[RelayCommand]
		protected async Task ExportAsync()
		{
			if (!ItemsLaba.Any())
			{
				await _dialogService.ShowMessage("Tidak ada data untuk diexport.");
				return;
			}

			var (start, end) = GetRange();

			await _exportService.ExportAsync(_allItems, start, end);
			await _dialogService.ShowMessage("Export berhasil.");
		}

		// ================= UTIL =================

		private static string FormatCurrency(decimal value)
			=> value.ToString("C", new CultureInfo("id-ID"));

		private void RaiseTotalsChanged()
		{
			OnPropertyChanged(nameof(TotalPenjualan));
			OnPropertyChanged(nameof(TotalBiaya));
			OnPropertyChanged(nameof(LabaBersih));
			OnPropertyChanged(nameof(TotalPenjualanText));
			OnPropertyChanged(nameof(TotalBiayaText));
			OnPropertyChanged(nameof(LabaBersihText));
		}
	}

}
