using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.CoreService;
using MicroCashDevWPFUI.Services.PembelianService;
using MicroCashDevWPFUI.Services.PenjualanService;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public abstract partial class NotaBaseViewModel : ObservableObject
	{
		protected readonly INotaService _notaService;
		protected readonly IDialogService _dialogService;
		protected readonly ISupplierService _supplierService;
		protected readonly INotaExportService _exportService;

		protected List<NotaItem> _allItems = new();

		[ObservableProperty]
		protected ObservableCollection<NotaItem> itemsNota = new();

		[ObservableProperty]
		private int currentPage = 1;

		[ObservableProperty]
		private int pageSize = 100;

		[ObservableProperty]
		private ObservableCollection<Supplier> itemsSupplier = new();

		public int TotalPages =>
			(int)Math.Ceiling((double)_allItems.Count / PageSize);

		public string PageInfo =>
			$"Halaman {CurrentPage} dari {TotalPages}";

		public decimal TotalNota =>
			ItemsNota.Sum(x => x.Total);

		public string TotalNotaText =>
			TotalNota.ToString("C", new CultureInfo("id-ID"));

		protected NotaBaseViewModel(
			INotaService notaService,
			IDialogService dialogService,
			ISupplierService supplierService,
			INotaExportService exportService)
		{
			_notaService = notaService;
			_dialogService = dialogService;
			_supplierService = supplierService;
			_exportService = exportService;

			ItemsNota.CollectionChanged += (_, __) =>
			{
				OnPropertyChanged(nameof(TotalNota));
				OnPropertyChanged(nameof(TotalNotaText));
			};
		}

		protected abstract (DateTime start, DateTime end) GetRange();

		protected virtual IEnumerable<Nota> ApplyFilter(IEnumerable<Nota> source)
		{
			return source;
		}

		protected void ApplyPaging()
		{
			ItemsNota.Clear();

			var pageData = _allItems
				.Skip((CurrentPage - 1) * PageSize)
				.Take(PageSize);

			foreach (var item in pageData)
				ItemsNota.Add(item);

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
			ItemsNota.Clear();

			ItemsSupplier = new ObservableCollection<Supplier>(
				await _supplierService.GetAllAsync()
			);

			var (start, end) = GetRange();
			var notaList = await _notaService.GetAllAsync(start, end);
			notaList = ApplyFilter(notaList).ToList();

			foreach (var nota in notaList)
			{
				var notaItem = new NotaItem(_notaService, nota.Id)
				{
					Id = nota.Id,
					Nomor = nota.Id,
					NomorFaktur = nota.Pembelian.NomorFaktur ?? "",
					TanggalDeadline = nota.TanggalDeadline,
					Supplier = nota.Pembelian.Supplier,
					Item = nota.Pembelian.Supplier?.Banks.Count ?? 0,
					Total = nota.Total,
					StatusPembayaran = nota.StatusPembayaran,
					Bank = new ObservableCollection<BankItem>(
						nota.Pembelian.Supplier?.Banks.Select(b => new BankItem
						{
							Id = b.Id,
							NamaBank = b.NamaBank,
							NomorRekening = b.NomorRekening
						}) ?? Enumerable.Empty<BankItem>()
					)
				};

				_allItems.Add(notaItem);
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
			if (!ItemsNota.Any())
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
