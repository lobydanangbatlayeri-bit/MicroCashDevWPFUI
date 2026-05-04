using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.CoreService;
using MicroCashDevWPFUI.Services.ProdukService;
using MicroCashDevWPFUI.Views.Controls;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public partial class TambahProdukViewModel : ObservableObject
	{
		private readonly ISatuanService _satuanService;
		private readonly IProdukService _produkService;
		private readonly IProdukSatuanService _produkSatuanService;
		private readonly IDialogService _dialogService;
        private readonly IProdukCacheService _produkCacheService;

        [ObservableProperty] private ObservableCollection<SatuanItem> itemsSatuan = new();
		[ObservableProperty] private ObservableCollection<ProdukSatuanItem> itemsSatuanProduk = new();
		[ObservableProperty] private bool isiPerBoxEnabled;
		[ObservableProperty] private bool isAllItemsSelected;
		[ObservableProperty] private string namaSatuanBaru = string.Empty;
		[ObservableProperty] private bool controlsEnabled = true;
		[ObservableProperty] private SatuanItem? selectedSatuan;
		[ObservableProperty] private decimal hargaJualInput = 0;
		[ObservableProperty] private int isiPerBoxInput = 1;
		[ObservableProperty] private string namaBarang = string.Empty;
		[ObservableProperty] private ObservableCollection<string> namaBarangList = new();
		[ObservableProperty] private bool isEditMode;

		public TambahProdukViewModel(
			ISatuanService satuanService,
			IProdukService produkService,
			IProdukSatuanService produkSatuanService,
			IDialogService dialogService,
			IProdukCacheService produkCacheService)
		{
			_satuanService = satuanService;
			_produkService = produkService;
			_produkSatuanService = produkSatuanService;
			_dialogService = dialogService;
			_produkCacheService = produkCacheService;

			_ = InitializeAsync();
		}

		private async Task InitializeAsync()
		{
			await LoadSatuanAsync();
			await LoadProdukAsync();
		}

		partial void OnNamaBarangChanged(string value) 
		{ 
			ItemsSatuanProduk.Clear(); 
			if (!NamaBarangList.Contains(value)) 
			{ 
				IsEditMode = false; 
				return; 
			} 
			_ = LoadProdukByNamaAsync(value); 
		}

		private async Task LoadProdukByNamaAsync(string namaBarang)
		{
			if (string.IsNullOrWhiteSpace(namaBarang)) return; 
			var produk = await _produkService.GetByNamaAsync(namaBarang); 
			if (produk == null) return; 
			IsEditMode = true; 
			// Clear data lama
			ItemsSatuanProduk.Clear(); 
			// Ambil satuan produk
			var produkSatuanList = await _produkSatuanService.GetByProdukIdAsync(produk.Id); 
			foreach (var ps in produkSatuanList) 
			{ 
				var satuan = ItemsSatuan.FirstOrDefault(s => s.Id == ps.SatuanId);
				if (satuan == null) continue;
				ItemsSatuanProduk.Add(new ProdukSatuanItem
				{
					ProdukId = ps.ProdukId,
					ProdukSatuanId = ps.Id,
					Satuan = satuan,
					IsiPerBox = ps.JumlahPerSatuan,
					HargaJual = ps.HargaJual
				});
			} 
		}


		private async Task LoadSatuanAsync()
		{
			ItemsSatuan.Clear();
			var data = await _satuanService.GetAllAsync();

			int nomor = 1;
			foreach (var s in data)
			{
				ItemsSatuan.Add(new SatuanItem
				{
					Nomor = nomor++,
					Id = s.Id,
					NamaSatuan = s.NamaSatuan
				});
			}
		}

		private async Task LoadProdukAsync()
		{
			NamaBarangList.Clear();
			var produkData = await _produkService.GetAllAsync();
			foreach (var p in produkData)
			{
				NamaBarangList.Add(p.NamaBarang);
			}
		}

		[RelayCommand]
		private async Task BukaTambahSatuan()
		{
			var view = new TambahSatuanDialog()
			{
				DataContext = this
			};

			var result = await _dialogService.ShowContentDialogAsync(
				"Tambah Satuan",
				view,
				"Simpan",
				"Batal");

			if (result == ContentDialogResult.Primary)
				await SimpanSatuan();

			NamaSatuanBaru = string.Empty;
		}

		[RelayCommand]
		async Task SimpanSatuan()
		{
			ControlsEnabled = false;

			try
			{
				if (!string.IsNullOrWhiteSpace(NamaSatuanBaru))
				{
					await _satuanService.AddAsync(new Satuan
					{
						NamaSatuan = NamaSatuanBaru
					});
					NamaSatuanBaru = string.Empty;
				}

				var itemsToUpdate = ItemsSatuan.Where(x => x.IsDirty && x.Id > 0).ToList();

				foreach (var item in itemsToUpdate)
				{
					await _satuanService.UpdateAsync(new Satuan
					{
						Id = item.Id,
						NamaSatuan = item.NamaSatuan
					});
					item.IsDirty = false;
				}

				await LoadSatuanAsync();
				return;
			}
			finally
			{
				ControlsEnabled = true;
			}
		}

		[RelayCommand]
		async Task DeleteSatuan(SatuanItem? item)
		{
			if (item == null)
				return;

			var result = System.Windows.MessageBox.Show(
				$"Yakin ingin menghapus satuan {item.NamaSatuan}?",
				"Konfirmasi Hapus",
				System.Windows.MessageBoxButton.YesNo,
				System.Windows.MessageBoxImage.Question);

			if (result != System.Windows.MessageBoxResult.Yes)
				return;

			try
			{
				ControlsEnabled = false;

				await _satuanService.DeleteAsync(item.Id);

				await LoadSatuanAsync();
			}
			catch (InvalidOperationException ex)
			{
				// 🔥 Tangkap error dari service (relasi masih dipakai)
				System.Windows.MessageBox.Show(
					ex.Message,
					"Tidak Bisa Menghapus",
					System.Windows.MessageBoxButton.OK,
					System.Windows.MessageBoxImage.Warning);
			}
			catch (Exception ex)
			{
				// Error tak terduga
				System.Windows.MessageBox.Show(
					ex.Message,
					"Error",
					System.Windows.MessageBoxButton.OK,
					System.Windows.MessageBoxImage.Error);
			}
			finally
			{
				ControlsEnabled = true;
			}
		}

		[RelayCommand]
		async Task TambahSatuanProduk()
		{
			if (SelectedSatuan is null)
			{
				await _dialogService.ShowMessage("Pilih satuan terlebih dahulu.");
				return;
			}

			if (HargaJualInput < 0)
			{
				await _dialogService.ShowMessage("Harga jual tidak boleh kurang dari 0.");
				return;
			}

			ItemsSatuanProduk.Add(new ProdukSatuanItem
			{
				Satuan = SelectedSatuan,
				IsiPerBox = IsiPerBoxEnabled ? IsiPerBoxInput : 1,
				HargaJual = HargaJualInput
			});

			SelectedSatuan = null;
			IsiPerBoxInput = 1;
			HargaJualInput = 0;
			IsiPerBoxEnabled = false;
		}

		[RelayCommand]
		void DeleteProdukSatuan(ProdukSatuanItem? item)
		{
			if (item != null)
				ItemsSatuanProduk.Remove(item);
		}

		partial void OnIsAllItemsSelectedChanged(bool value)
		{
			foreach (var item in ItemsSatuanProduk)
				item.IsSelected = value;
		}

		[RelayCommand]
		async Task SimpanProduk()
		{
			if (string.IsNullOrWhiteSpace(NamaBarang))
			{
				await _dialogService.ShowMessage("Nama barang tidak boleh kosong.");
				return;
			}

			if (ItemsSatuanProduk.Count == 0)
			{
				await _dialogService.ShowMessage("Tambahkan minimal satu satuan!");
				return;
			}

			try
			{
				ControlsEnabled = false;

				// 🔍 cek apakah produk sudah ada
				var existingProduk = await _produkService.GetByNamaAsync(NamaBarang);

				Produk produk;

				if (existingProduk != null)
				{
					var result = await _dialogService.ShowConfirmation(
						"Konfirmasi Update",
						$"Produk \"{NamaBarang}\" sudah ada.\nYakin ingin memperbarui data produk ini?"
					);

					if (result != ContentDialogResult.Primary)
						return;

					produk = existingProduk;
					IsEditMode = true;   // 🔥 ini penting
				}
				else
				{
					produk = await _produkService.AddAsync(new Produk
					{
						NamaBarang = NamaBarang
					});

					IsEditMode = false;  // 🔥 produk baru
				}

				// 🔁 SINKRON SATUAN
				await SinkronProdukSatuanAsync(produk.Id);

                await _produkCacheService.AddOrUpdateAsync(produk);

                NamaBarang = string.Empty;
				ItemsSatuanProduk.Clear();
				await LoadProdukAsync();

				await _dialogService.ShowMessage("Produk berhasil disimpan!");
			}
			finally
			{
				ControlsEnabled = true;
			}
		}

		private async Task SinkronProdukSatuanAsync(int produkId)
		{
			var existingSatuans =
				await _produkSatuanService.GetByProdukIdAsync(produkId);

			// 1️⃣ UPDATE & INSERT
			foreach (var item in ItemsSatuanProduk)
			{
				if (item.ProdukSatuanId > 0)
				{
					var existing = existingSatuans
						.FirstOrDefault(x => x.Id == item.ProdukSatuanId);

					if (existing != null)
					{
						existing.JumlahPerSatuan = item.IsiPerBox;
						existing.HargaJual = item.HargaJual;

						await _produkSatuanService.UpdateAsync(existing);
					}
				}
				else
				{
					await _produkSatuanService.AddAsync(new ProdukSatuan
					{
						ProdukId = produkId,
						SatuanId = item.Satuan!.Id,
						JumlahPerSatuan = item.IsiPerBox,
						HargaJual = item.HargaJual
					});
				}
			}

			// 2️⃣ DELETE: hanya yang BELUM PUNYA BATCH
			var toDelete = existingSatuans
				.Where(x => !ItemsSatuanProduk
					.Any(i => i.ProdukSatuanId == x.Id))
				.ToList();

			foreach (var del in toDelete)
				await _produkSatuanService.DeleteAsync(del.Id);
		}

		[RelayCommand]
		async Task Cancel()
		{
			NamaBarang = string.Empty;
			ItemsSatuanProduk.Clear();
			SelectedSatuan = null;
			IsiPerBoxInput = 1;
			HargaJualInput = 0;
			IsiPerBoxEnabled = false;

			await LoadProdukAsync();
		}
	}
}