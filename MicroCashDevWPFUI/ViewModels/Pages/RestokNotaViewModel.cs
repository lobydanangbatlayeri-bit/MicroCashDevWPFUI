using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.CoreService;
using MicroCashDevWPFUI.Services.PembelianService;
using MicroCashDevWPFUI.Services.ProdukService;
using MicroCashDevWPFUI.Views.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using Wpf.Ui.Controls;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public partial class RestokNotaViewModel : ObservableObject
	{
		private readonly IDialogService _dialogService;
		private readonly ISupplierService _supplierService;
		private readonly IRestokService _restokService;
		private readonly IPembelianService _pembelianService;
        private readonly IProdukCacheService _produkCacheService;

        [ObservableProperty]
		private ObservableCollection<SupplierItem> itemsSupplier = new();
		[ObservableProperty]
		private SupplierItem? selectedSupplier;
		[ObservableProperty]
		private string? nomorNota;
		[ObservableProperty]
		private ObservableCollection<string> nomorNotaList = new();
		[ObservableProperty]
		private DateTime? tanggalRestok = DateTime.Now;
		[ObservableProperty]
		private decimal total = 0;
		[ObservableProperty]
		private string? namaBarang;
		[ObservableProperty]
		private ObservableCollection<string> namaBarangList = new();
		[ObservableProperty]
		private int tambahStock = 0;
		[ObservableProperty]
		private decimal hargaBeli = 0;
		[ObservableProperty]
		private decimal subTotal = 0;
		[ObservableProperty]
		private string? nomorBatch;
		[ObservableProperty]
		private DateTime? tanggalMasuk = DateTime.Now;
		[ObservableProperty]
		private DateTime? tanggalKadarluasa;
		[ObservableProperty]
		private int sisaStok = 0;
		[ObservableProperty]
		private string? namaSatuan;
		[ObservableProperty]
		private ObservableCollection<ProdukStockItem> itemsStockProduk = new();
		[ObservableProperty]
		private string namaSupplierBaru = string.Empty;
		[ObservableProperty]
		private string alamatSupplierBaru = string.Empty;
		[ObservableProperty] 
		private bool controlsEnabled = true;

		private ProdukSatuan? _selectedProdukSatuan;
		private bool _initialized;
        private CancellationTokenSource? _cts;

        public RestokNotaViewModel(
			IDialogService dialogService,
			ISupplierService supplierService,
			IRestokService restokService,
			IPembelianService pembelianService,
			IProdukCacheService produkCacheService)
		{
			_dialogService = dialogService;
			_supplierService = supplierService;
			_restokService = restokService;
			_pembelianService = pembelianService;
			_produkCacheService = produkCacheService;

            _produkCacheService.OnCacheUpdated += OnCacheUpdatedHandler;
        }

        private void OnCacheUpdatedHandler()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                _ = LoadProdukAsync();
            });
        }

        public async Task EnsureInitializedAsync()
		{
			if (_initialized)
				return;

			_initialized = true;

            await _produkCacheService.EnsureLoadedAsync();
            await LoadSupplierAsync();
			await LoadProdukAsync();
		}

        partial void OnNamaBarangChanged(string? value)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            _ = DebounceLoadAsync(value, token);
        }

        private async Task DebounceLoadAsync(string? value, CancellationToken token)
        {
            try
            {
                await Task.Delay(300, token);

                if (token.IsCancellationRequested)
                    return;

                await LoadSatuanBeliAsync(value ?? string.Empty);
            }
            catch (TaskCanceledException)
            {
                // aman, abaikan
            }
        }

        partial void OnTambahStockChanged(int value)
		{
			HitungSubTotal();
		}


		partial void OnHargaBeliChanged(decimal value)
		{
			HitungSubTotal();
		}

		partial void OnSelectedSupplierChanged(SupplierItem? value)
		{
			NomorNotaList.Clear();
			NomorNota = string.Empty;

			if (value == null)
				return;

			_ = LoadNomorNotaAsync(value.Id);
		}

		private async Task LoadNomorNotaAsync(int supplierId)
		{
			var list = await _pembelianService
				.GetNomorFakturBySupplierAsync(supplierId);

			NomorNotaList = new ObservableCollection<string>(list);
		}

		private async Task LoadSupplierAsync()
		{
			ItemsSupplier.Clear();
			var data = await _supplierService.GetAllAsync();

			int nomor = 1;
			foreach (var s in data)
			{
				ItemsSupplier.Add(new SupplierItem
				{
					Nomor = nomor++,
					Id = s.Id,
					NamaSupplier = s.NamaSupplier,
					Alamat = s.Alamat
				});
			}
			SelectedSupplier = ItemsSupplier.FirstOrDefault();
		}

        private Task LoadProdukAsync()
        {
            var list = _produkCacheService.GetAll().Select(p => p.NamaBarang).ToList();

            Debug.WriteLine($"JUMLAH PRODUK: {list.Count}");

            NamaBarangList.Clear();
            foreach (var item in list)
                NamaBarangList.Add(item);

            return Task.CompletedTask;
        }

        private async Task LoadSatuanBeliAsync(string namaBarang)
		{
			NamaSatuan = string.Empty;
			SisaStok = 0;
			_selectedProdukSatuan = null;

			if (string.IsNullOrWhiteSpace(namaBarang))
				return;

            var produk = _produkCacheService.Find(namaBarang);

            if (produk == null)
				return;

            var satuans = await _produkCacheService.GetSatuanAsync(produk.Id);

            var satuanBeli = satuans
				.OrderByDescending(s => s.JumlahPerSatuan)
				.First();

			NamaSatuan = satuanBeli.Satuan!.NamaSatuan;
			_selectedProdukSatuan = satuanBeli;

			// stok lama
			var satuanDasar = satuans.OrderBy(s => s.JumlahPerSatuan).First();
			int stokDasar = satuanDasar.ProdukBatchs?.Sum(b => b.Stok) ?? 0;
			SisaStok = stokDasar / satuanBeli.JumlahPerSatuan;
		}

		private void HitungSubTotal()
		{
			SubTotal = TambahStock * HargaBeli;
		}

		public decimal SaranHargaPenjualan
		{
			get
			{
				if (HargaBeli <= 0)
					return 0;

				return Math.Ceiling(((HargaBeli * 1.3m) + 0.11m) / 1000m) * 1000m;
			}
		}

		private void RecalculateTotal()
		{
			Total = ItemsStockProduk.Sum(x => x.SubTotal);
		}

		[RelayCommand]
		private async Task BukaTambahSupplier()
		{
			var view = new TambahSupplierDialog()
			{
				DataContext = this
			};

			var result = await _dialogService.ShowContentDialogAsync(
				"Tambah Supplier",
				view,
				"Simpan",
				"Batal");

			if (result == ContentDialogResult.Primary)
				await SimpanSupplier();

			NamaSupplierBaru = string.Empty;
			AlamatSupplierBaru = string.Empty;
		}

		[RelayCommand]
		async Task SimpanSupplier()
		{
			ControlsEnabled = false;
			try
			{
				if (!string.IsNullOrWhiteSpace(NamaSupplierBaru))
				{
					await _supplierService.AddAsync(new Supplier
					{
						NamaSupplier = NamaSupplierBaru,
						Alamat = AlamatSupplierBaru
					});
					NamaSupplierBaru = string.Empty;
					AlamatSupplierBaru = string.Empty;
				}

				var itemsToUpdate = ItemsSupplier.Where(x => x.IsDirty && x.Id > 0).ToList();

				foreach (var item in itemsToUpdate)
				{
					await _supplierService.UpdateAsync(new Supplier
					{
						Id = item.Id,
						NamaSupplier = item.NamaSupplier,
						Alamat = item.Alamat
					});
					item.IsDirty = false;
				}

				await LoadSupplierAsync();
				return;
			}
			finally
			{
				ControlsEnabled = true;
			}
		}

		[RelayCommand]
		async Task DeleteSupplier(SupplierItem? item)
		{
			if (item == null)
				return;

			var result = System.Windows.MessageBox.Show(
				$"Yakin ingin menghapus supplier {item.NamaSupplier}?",
				"Konfirmasi Hapus",
				System.Windows.MessageBoxButton.YesNo,
				System.Windows.MessageBoxImage.Question);

			if (result != System.Windows.MessageBoxResult.Yes)
				return;

			try
			{
				ControlsEnabled = false;

				var success = await _supplierService.DeleteAsync(item.Id);

				if (!success)
				{
					System.Windows.MessageBox.Show(
						"Supplier sudah digunakan dalam transaksi dan tidak bisa dihapus.",
						"Informasi",
						System.Windows.MessageBoxButton.OK,
						System.Windows.MessageBoxImage.Information);

					return;
				}

				await LoadSupplierAsync();
			}
			finally
			{
				ControlsEnabled = true;
			}
		}

		[RelayCommand]
		private async Task TambahObat()
		{
			if (_selectedProdukSatuan == null)
			{
				await _dialogService.ShowMessage("Silakan pilih barang terlebih dahulu.");
				return;
			}

			if (TambahStock <= 0)
			{
				await _dialogService.ShowMessage("Jumlah stok harus lebih dari 0.");
				return;
			}

			if (HargaBeli <= 0)
			{
				await _dialogService.ShowMessage("Harga beli harus lebih dari 0.");
				return;
			}

			if (NomorBatch == null || NomorBatch.Trim().Length == 0)
			{
				await _dialogService.ShowMessage("Nomor batch wajib diisi.");
				return;
			}

			// 🔔 KONFIRMASI TANGGAL KADALUARSA
			if (TanggalKadarluasa == null)
			{
				var result = await _dialogService.ShowConfirmation(
					"Konfirmasi",
					"Yakin tidak menambahkan tanggal kadaluarsa?"
				);

				if (result != ContentDialogResult.Primary)
					return; // User pilih "Tidak"
			}

			var item = new ProdukStockItem
			{
				ProdukSatuanId = _selectedProdukSatuan.Id,
				NamaBarang = NamaBarang,
				Jumlah = TambahStock,
				HargaBeli = HargaBeli,
				SubTotal = SubTotal,
				NomorBatch = NomorBatch,
				TanggalMasuk = TanggalMasuk,
				TanggalKadarluasa = TanggalKadarluasa
			};

			ItemsStockProduk.Add(item);

			RecalculateTotal();

			// Reset
			NamaBarang = string.Empty;
			TambahStock = 0;
			SubTotal = 0;
			HargaBeli = 0;
			NomorBatch = string.Empty;
			TanggalKadarluasa = null;
		}

		[RelayCommand]
		private async Task DeleteProdukStock(ProdukStockItem? item)
		{
			if (item == null)
				return;

			var result = await _dialogService.ShowConfirmation(
				"Konfirmasi Hapus",
				$"Yakin ingin menghapus {item.NamaBarang}?"
			);

			if (result != ContentDialogResult.Primary)
				return;

			// Hapus dari list
			ItemsStockProduk.Remove(item);

			RecalculateTotal();
		}

		[RelayCommand]
		private async Task Simpan()
		{
			// 1️⃣ CEK ITEM
			if (!ItemsStockProduk.Any())
			{
				await _dialogService.ShowMessage("Belum ada item yang direstok.");
				return;
			}

			// 2️⃣ CEK SUPPLIER
			if (SelectedSupplier == null)
			{
				await _dialogService.ShowMessage("Supplier wajib dipilih.");
				return;
			}

			// 3️⃣ CEK NOMOR NOTA
			if (string.IsNullOrWhiteSpace(NomorNota))
			{
				await _dialogService.ShowMessage("Nomor nota wajib diisi.");
				return;
			}

			if (NomorNotaList.Contains(NomorNota))
			{
				await _dialogService.ShowMessage(
					"Nomor nota sudah pernah digunakan untuk supplier ini.");
				return;
			}

			// 4️⃣ MAP ITEM
			var items = ItemsStockProduk.Select(x => new RestokNotaItemDto
			{
				ProdukSatuanId = x.ProdukSatuanId,
				Jumlah = x.Jumlah,
				HargaBeli = x.HargaBeli,
				SubTotal = x.SubTotal,
				TanggalMasuk = x.TanggalMasuk ?? DateTime.Now,
				TanggalKadarluasa = x.TanggalKadarluasa,
				NomorBatch = x.NomorBatch
			}).ToList();

			// 5️⃣ SIMPAN
			await _restokService.RestokNotaAsync(
				SelectedSupplier.Id,
				items,
				Total,
				NomorNota,
				TanggalRestok ?? DateTime.Now
			);

			await _dialogService.ShowMessage("Stok berhasil ditambahkan.");

			// 6️⃣ RESET UI
			ResetForm();
		}

		[RelayCommand]
		private async Task Batal()
		{
			// Jika belum ada perubahan → langsung reset
			if (!ItemsStockProduk.Any()
				&& string.IsNullOrWhiteSpace(NomorNota)
				&& TambahStock == 0
				&& HargaBeli == 0)
			{
				ResetForm();
				return;
			}

			var result = await _dialogService.ShowConfirmation(
				"Konfirmasi",
				"Yakin ingin membatalkan? Semua data yang belum disimpan akan hilang."
			);

			if (result != ContentDialogResult.Primary)
				return;

			ResetForm();
		}

		private void ResetForm()
		{
			// Header
			NomorNota = string.Empty;
			TanggalRestok = DateTime.Now;
			Total = 0;

			// Supplier
			SelectedSupplier = ItemsSupplier.FirstOrDefault();

			// Item
			ItemsStockProduk.Clear();

			// Input barang
			NamaBarang = string.Empty;
			NamaSatuan = string.Empty;
			TambahStock = 0;
			HargaBeli = 0;
			SubTotal = 0;
			NomorBatch = string.Empty;
			TanggalKadarluasa = null;
			TanggalMasuk = DateTime.Now;
			SisaStok = 0;

			// Internal state
			_selectedProdukSatuan = null;
		}

	}
}
