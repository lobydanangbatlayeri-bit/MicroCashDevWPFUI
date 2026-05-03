using MicroCashDevWPFUI.Api.Infrastructure;
using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Matching.Interfaces;
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
		private readonly IProdukService _produkService;
		private readonly IProdukSatuanService _produkSatuanService;
		private readonly IRestokService _restokService;
		private readonly ScanNotaMemoryStore _store;
		private readonly ISupplierMatcher _supplierMatcher;
		private readonly IProductMatcher _productMatcher;
		private readonly IPembelianService _pembelianService;

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
		private Queue<RestokNotaScanItemDto> _scanQueue = new();
		private RestokNotaScanItemDto? _currentScanItem;
		private bool _isScanMode = false;
		private List<ProductItem> _productItems = new();

		public RestokNotaViewModel(
			IDialogService dialogService,
			ISupplierService supplierService,
			IProdukService produkService,
			IProdukSatuanService produkSatuanService,
			IRestokService restokService,
			ScanNotaMemoryStore store,
			ISupplierMatcher supplierMatcher,
			IProductMatcher productMatcher,
			IPembelianService pembelianService)
		{
			_dialogService = dialogService;
			_supplierService = supplierService;
			_produkService = produkService;
			_produkSatuanService = produkSatuanService;
			_restokService = restokService;
			_store = store;
			_supplierMatcher = supplierMatcher;
			_productMatcher = productMatcher;
			_pembelianService = pembelianService;
		}

		public async Task EnsureInitializedAsync()
		{
			if (_initialized)
				return;

			_initialized = true;
			await LoadSupplierAsync();
			await LoadProdukAsync();
		}

		public async Task RefreshProdukAsync()
		{
			await LoadProdukAsync();
		}

		partial void OnNamaBarangChanged(string? value)
		{
			_ = LoadSatuanBeliAsync(value ?? string.Empty);
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

		private async Task LoadProdukAsync()
		{
			NamaBarangList.Clear();
			_productItems.Clear();

			var produkData = await _produkService.GetAllAsync();

			foreach (var p in produkData)
			{
				NamaBarangList.Add(p.NamaBarang);

				_productItems.Add(new ProductItem
				{
					Id = p.Id,
					NamaBarang = p.NamaBarang
				});
			}
		}

		private async Task LoadSatuanBeliAsync(string namaBarang)
		{
			NamaSatuan = string.Empty;
			SisaStok = 0;
			_selectedProdukSatuan = null;

			if (string.IsNullOrWhiteSpace(namaBarang))
				return;

			var produk = (await _produkService.GetAllAsync())
				.FirstOrDefault(p => p.NamaBarang.Equals(namaBarang, StringComparison.OrdinalIgnoreCase));

			if (produk == null)
				return;

			var satuans = await _produkSatuanService.GetByProdukIdAsync(produk.Id);

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
			if (_isScanMode)
				return;

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

			if (_isScanMode)
			{
				await LoadNextScanItem();
			}
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
			_isScanMode = false;
		}

		public async Task LoadFromScanAsync()
		{
			var data = _store.Get();
			if (data == null)
				return;

			_isScanMode = true;

			var matched = _supplierMatcher.FindClosest(ItemsSupplier, data.SupplierNama);
			SelectedSupplier = matched ?? ItemsSupplier.FirstOrDefault();

			NomorNota = data.NomorNota;
			TanggalRestok = data.TanggalRestok;
			TanggalMasuk = data.TanggalRestok;

			Total = data.Total;

			_scanQueue = new Queue<RestokNotaScanItemDto>(data.Items);

			// load item pertama ke form
			await LoadNextScanItem();

			_store.Clear();
		}

		private async Task LoadNextScanItem()
		{
			if (!_isScanMode)
				return;

			if (!_scanQueue.Any())
			{
				_currentScanItem = null;
				_isScanMode = false;
				await _dialogService.ShowMessage("Semua item scan sudah dimasukkan.");
				return;
			}

			_currentScanItem = _scanQueue.Dequeue();

			// tampilkan hasil OCR
			NamaBarang = _currentScanItem.NamaBarang;

			// 🔥 MATCH PRODUK DI SINI
			var matchedProduct = _productMatcher.FindClosest(
				_productItems,
				_currentScanItem.NamaBarang);

			if (matchedProduct != null)
			{
				NamaBarang = matchedProduct.NamaBarang;
			}
			else
			{
				// biarkan hasil OCR, user pilih manual
			}

			TambahStock = _currentScanItem.Jumlah;
			HargaBeli = _currentScanItem.HargaBeli;
			SubTotal = _currentScanItem.SubTotal;
			NomorBatch = _currentScanItem.NomorBatch;
			TanggalKadarluasa = _currentScanItem.TanggalKadarluasa;
		}

		[RelayCommand]
		private async Task TerimaScanNota()
		{
			var data = _store.Get();

			if (data == null)
			{
				Debug.WriteLine("Data scan kosong.");
				return;
			}

			var detail = new StringBuilder();
			detail.AppendLine("===== HEADER =====");
			detail.AppendLine($"Supplier     : {data.SupplierNama}");
			detail.AppendLine($"Nomor Nota   : {data.NomorNota}");
			detail.AppendLine($"Tanggal      : {data.TanggalRestok}");
			detail.AppendLine($"Total        : {data.Total}");
			detail.AppendLine("");
			detail.AppendLine("===== ITEMS =====");

			foreach (var item in data.Items)
			{
				detail.AppendLine("----------------------");
				detail.AppendLine($"Nama Barang : {item.NamaBarang}");
				detail.AppendLine($"Jumlah      : {item.Jumlah}");
				detail.AppendLine($"Harga Beli  : {item.HargaBeli}");
				detail.AppendLine($"SubTotal    : {item.SubTotal}");
				detail.AppendLine($"Batch       : {item.NomorBatch}");
				detail.AppendLine($"Exp         : {item.TanggalKadarluasa}");
			}

			// Cetak ke Debug Console
			Debug.WriteLine(detail.ToString());

			await LoadFromScanAsync();
		}


	}
}
