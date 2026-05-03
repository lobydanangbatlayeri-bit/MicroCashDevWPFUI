using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.CoreService;
using MicroCashDevWPFUI.Services.ProdukService;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public partial class ProdukViewModel : ObservableObject
	{
		private readonly IProdukService _produkService;
		private readonly IProdukSatuanService _produkSatuanService;
		private readonly IDialogService _dialogService;

		private readonly List<ProdukItem> _allItems = new();
		private List<ProdukItem> _currentSource = new();
		private readonly Dictionary<int, string> _originalNames = new();

		[ObservableProperty]
		private string namaBarang = string.Empty;

		[ObservableProperty]
		private ObservableCollection<string> namaBarangList = new();

		[ObservableProperty]
		private ObservableCollection<string> satuanList = new();

		[ObservableProperty]
		private int totalStok = 0;

		[ObservableProperty]
		private decimal hargaJual = 0;

		[ObservableProperty]
		private ObservableCollection<ProdukItem> itemsProduk = new();

		[ObservableProperty]
		private int currentPage = 1;

		[ObservableProperty]
		private int totalPages = 1;

		[ObservableProperty]
		private int pageSize = 30;

		public string PageInfo => $"Page {CurrentPage} of {TotalPages}";

		public ProdukViewModel(
			IProdukService produkService,
			IProdukSatuanService produkSatuanService,
			IDialogService dialogService)
		{
			_produkService = produkService;
			_produkSatuanService = produkSatuanService;			
			_dialogService = dialogService;

			NamaBarangList = new ObservableCollection<string>();
			SatuanList = new ObservableCollection<string>();
			ItemsProduk = new ObservableCollection<ProdukItem>();

			_ = LoadDataAsync();
		}

		private async Task LoadDataAsync()
		{
			try
			{
				var produkSatuans = await _produkSatuanService.GetAllAsync();

				var produkGroups = produkSatuans
					.GroupBy(ps => ps.Produk.Id)
					.ToList();

				_allItems.Clear();
				_originalNames.Clear();
				NamaBarangList.Clear();

				foreach (var grp in produkGroups)
				{
					var produk = grp.First().Produk;
					var produkName = produk.NamaBarang;
					var produkId = produk.Id;

					var unitsForProduct = grp
						.Select(ps => ps.Satuan.NamaSatuan)
						.Distinct()
						.ToList();

					var item = new ProdukItem
					{
						ProdukId = produkId,
						NamaBarang = produkName,
						Satuan = unitsForProduct.FirstOrDefault() ?? string.Empty,
						SatuanList = new ObservableCollection<string>(unitsForProduct),
						ProdukSatuans = new ObservableCollection<ProdukSatuan>(grp)
					};

					item.Initialize(); // ✅ setelah semua properti siap

					NamaBarangList.Add(produkName);
					_allItems.Add(item);
					_originalNames[produkId] = produkName;
				}

				_currentSource = _allItems;
				CurrentPage = 1;
				UpdatePaging();
			}
			catch (Exception ex)
			{
				await _dialogService.ShowMessage(ex.Message);
			}
		}

		partial void OnNamaBarangChanged(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				_currentSource = _allItems;
				SatuanList = new ObservableCollection<string>();
				CurrentPage = 1;
				UpdatePaging();
				return;
			}

			_currentSource = _allItems
				.Where(i => i.NamaBarang != null && i.NamaBarang.Contains(value, StringComparison.OrdinalIgnoreCase))
				.ToList();

			var units = _currentSource
				.SelectMany(i => i.SatuanList ?? Enumerable.Empty<string>())
				.Distinct()
				.ToList();

			SatuanList = new ObservableCollection<string>(units);

			CurrentPage = 1;
			UpdatePaging();
		}

		[RelayCommand(CanExecute = nameof(CanPrevPage))]
		private void PrevPage()
		{
			if (CurrentPage > 1) CurrentPage--;
		}

		private bool CanPrevPage() => CurrentPage > 1;

		[RelayCommand(CanExecute = nameof(CanNextPage))]
		private void NextPage()
		{
			if (CurrentPage < TotalPages) CurrentPage++;
		}

		private bool CanNextPage() => CurrentPage < TotalPages;

		partial void OnCurrentPageChanged(int value)
		{
			UpdatePaging();
			PrevPageCommand?.NotifyCanExecuteChanged();
			NextPageCommand?.NotifyCanExecuteChanged();
			OnPropertyChanged(nameof(PageInfo));
		}

		partial void OnTotalPagesChanged(int value)
		{
			PrevPageCommand?.NotifyCanExecuteChanged();
			NextPageCommand?.NotifyCanExecuteChanged();
			OnPropertyChanged(nameof(PageInfo));
		}

		private void UpdatePaging()
		{
			if (_currentSource == null) _currentSource = _allItems;

			var total = Math.Max(0, _currentSource?.Count ?? 0);
			TotalPages = total == 0 ? 1 : (int)Math.Ceiling(total / (double)PageSize);

			if (CurrentPage < 1) CurrentPage = 1;
			if (CurrentPage > TotalPages) CurrentPage = TotalPages;

			var pageItems = (_currentSource ?? Enumerable.Empty<ProdukItem>())
				.Skip((CurrentPage - 1) * PageSize)
				.Take(PageSize)
				.ToList();

			ItemsProduk = new ObservableCollection<ProdukItem>(pageItems);
		}

		private async Task SimpanProdukAsync(ProdukItem item)
		{
			var produk = await _produkService.GetByIdAsync(item.ProdukId);
			if (produk == null) return;

			await UpdateNamaProdukAsync(item, produk);
			await UpdateHargaAsync(item);
		}

		private async Task UpdateNamaProdukAsync(ProdukItem item, Produk produk)
		{
			if (produk.NamaBarang == item.NamaBarang)
				return;

			_originalNames.TryGetValue(item.ProdukId, out var originalName);

			produk.NamaBarang = item.NamaBarang;
			await _produkService.UpdateAsync(produk);

			var idx = !string.IsNullOrEmpty(originalName)
				? NamaBarangList.IndexOf(originalName)
				: NamaBarangList.IndexOf(produk.NamaBarang);

			if (idx >= 0)
				NamaBarangList[idx] = item.NamaBarang;

			_originalNames[item.ProdukId] = item.NamaBarang;
		}

		private async Task UpdateHargaAsync(ProdukItem item)
		{
			var selectedPs = item.ProdukSatuans
				.FirstOrDefault(ps => ps.Satuan?.NamaSatuan == item.Satuan);

			if (selectedPs == null)
				return;

			if (selectedPs.HargaJual != item.HargaJual)
			{
				selectedPs.HargaJual = item.HargaJual;
				await _produkSatuanService.UpdateAsync(selectedPs);
			}
		}

		private void ResetPaging()
		{
			_currentSource = _allItems;
			CurrentPage = 1;
			UpdatePaging();
		}

		[RelayCommand]
		private async Task Simpan()
		{
			try
			{
				var dirtyItems = _allItems.Where(i => i.IsDirty).ToList();
				if (!dirtyItems.Any())
				{
					await _dialogService.ShowMessage("Tidak ada perubahan.");
					return;
				}

				var debugLines = new List<string>();

				foreach (var item in dirtyItems)
				{
					await SimpanProdukAsync(item);
					item.RefreshAfterSave();
					item.CaptureOriginalState();
				}

				ResetPaging();

				await _dialogService.ShowMessage("Perubahan produk berhasil disimpan.");
			}
			catch (Exception ex)
			{
				await _dialogService.ShowMessage(ex.Message);
			}
		}

		[RelayCommand]
		private async Task CancelAsync()
		{
			foreach (var item in _allItems)
			{
				item.Restore();
			}

			_currentSource = _allItems;
			CurrentPage = 1;
			UpdatePaging();

			await _dialogService.ShowMessage("Perubahan dibatalkan.");
		}

		[RelayCommand]
		private async Task DeleteProduk(ProdukItem item)
		{
			if (item == null)
				return;

			var result = await _dialogService.ShowConfirmation(
				"Konfirmasi Hapus",
				$"Hapus produk '{item.NamaBarang}' ?");

			if (result != Wpf.Ui.Controls.ContentDialogResult.Primary)
				return;

			await _produkService.DeleteAsync(item.ProdukId);

			_allItems.Remove(item);
			ResetPaging();

			await _dialogService.ShowMessage("Produk berhasil dihapus.");
		}

	}
}
