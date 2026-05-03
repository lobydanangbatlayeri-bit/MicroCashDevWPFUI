using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.CoreService;
using MicroCashDevWPFUI.Services.PembelianService;
using MicroCashDevWPFUI.Services.ProdukService;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public abstract partial class PembelianBaseViewModel : ObservableObject
	{
		protected readonly IPembelianService _pembelianService;
		protected readonly IDetailPembelianService _detailService;
		protected readonly IProdukBatchService _produkBatchService;
		protected readonly IPembelianExportService _exportService;
		protected readonly IDialogService _dialogService;
		protected readonly ISupplierService _supplierService;
		protected readonly ISatuanService _satuanService;

		protected List<PembelianItem> _allItems = new();

		[ObservableProperty]
		protected ObservableCollection<PembelianItem> itemsPembelian = new();

		[ObservableProperty]
		private int currentPage = 1;

		[ObservableProperty]
		private int pageSize = 100;

		[ObservableProperty]
		private ObservableCollection<Supplier> itemsSupplier = new();

		[ObservableProperty]
		private ObservableCollection<Satuan> itemsSatuan = new();

		public int TotalPages =>
			(int)Math.Ceiling((double)_allItems.Count / PageSize);

		public string PageInfo =>
			$"Halaman {CurrentPage} dari {TotalPages}";

		public decimal TotalPembelian =>
			ItemsPembelian.Sum(x => x.Total);

		public string TotalPembelianText =>
			TotalPembelian.ToString("C", new CultureInfo("id-ID"));

		protected PembelianBaseViewModel(
			IPembelianService pembelianService,
			IDetailPembelianService detailService,
			IProdukBatchService produkBatchService,
			IPembelianExportService exportService,
			IDialogService dialogService,
			ISupplierService supplierService,
			ISatuanService satuanService)
		{
			_pembelianService = pembelianService;
			_detailService = detailService;
			_produkBatchService = produkBatchService;
			_exportService = exportService;
			_dialogService = dialogService;
			_supplierService = supplierService;
			_satuanService = satuanService;

			ItemsPembelian.CollectionChanged += (_, __) =>
			{
				OnPropertyChanged(nameof(TotalPembelian));
				OnPropertyChanged(nameof(TotalPembelianText));
			};
		}

		protected abstract (DateTime start, DateTime end) GetRange();

		protected virtual IEnumerable<Pembelian> ApplyFilter(IEnumerable<Pembelian> source)
		{
			return source;
		}

		protected void ApplyPaging()
		{
			ItemsPembelian.Clear();

			var pageData = _allItems
				.Skip((CurrentPage - 1) * PageSize)
				.Take(PageSize);

			foreach (var item in pageData)
				ItemsPembelian.Add(item);

			OnPropertyChanged(nameof(PageInfo));
		}

		private bool CanNextPage() => CurrentPage < TotalPages;

		partial void OnCurrentPageChanged(int value)
		{
			OnPropertyChanged(nameof(PageInfo));
			PrevPageCommand.NotifyCanExecuteChanged();
			NextPageCommand.NotifyCanExecuteChanged();
		}

		private bool CanPrevPage() => CurrentPage > 1;

		[RelayCommand]
		protected async Task LoadDataAsync()
		{
			_allItems.Clear();
			ItemsPembelian.Clear();

			ItemsSupplier = new ObservableCollection<Supplier>(
				await _supplierService.GetAllAsync()
			);

			ItemsSatuan = new ObservableCollection<Satuan>(
				await _satuanService.GetAllAsync()
			);


			var (start, end) = GetRange();
			var pembelianList = await _pembelianService.GetRiwayatAsync(start, end);
			pembelianList = ApplyFilter(pembelianList).ToList();

			foreach (var pembelian in pembelianList)
			{
				var details = await _detailService.GetByPembelianIdAsync(pembelian.Id);
				var detailDTOs = new ObservableCollection<DetailPembelianItem>();

				foreach (var detail in details)
				{
					var batchs = await _produkBatchService.GetByDetailPembelianIdAsync(detail.Id);

					var detailItem = new DetailPembelianItem(_detailService, detail.Id)
					{
						Id = detail.Id,
						Produk = detail.ProdukSatuan?.Produk?.NamaBarang ?? "",
						Satuan = detail.ProdukSatuan?.Satuan,
						Jumlah = detail.Jumlah,
						HargaBeli = detail.HargaBeli,
						Subtotal = detail.Subtotal
					};

					foreach (var batch in batchs)
					{
						detailItem.ProdukBatchs.Add(new ProdukBatchItem(_produkBatchService, batch.Id)
						{
							Id = batch.Id,
							BatchNumber = batch.BatchNumber ?? "",
							TanggalKadarluasa = batch.TanggalKadarluasa ?? DateTime.MinValue,
							Stok = batch.Stok
						});
					}

					detailDTOs.Add(detailItem);
				}

				_allItems.Add(new PembelianItem(_pembelianService, pembelian.Id)
				{
					Id = pembelian.Id,
					Nomor = pembelian.Id,
					Tanggal = pembelian.Tanggal,
					NomorFaktur = pembelian.NomorFaktur ?? "",
					Supplier = pembelian.Supplier,
					Item = details.Count,
					Total = pembelian.Total,
					DetailPembelians = detailDTOs
				});
			}

			CurrentPage = 1;
			ApplyPaging();
		}

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

		[RelayCommand]
		protected async Task ExportAsync()
		{
			if (!ItemsPembelian.Any())
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
