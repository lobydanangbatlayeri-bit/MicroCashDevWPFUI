using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Services;
using MicroCashDevWPFUI.Services.CoreService;
using MicroCashDevWPFUI.Services.PenjualanService;
using MicroCashDevWPFUI.Services.ProdukService;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Wpf.Ui.Controls;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public partial class PenjualanViewModel : ObservableObject
	{
		private readonly IProdukService _produkService;
		private readonly IProdukSatuanService _produkSatuanService;
		private readonly ISatuanService _satuanService;
		private readonly IDialogService _dialogService;
		private readonly IPenjualanService _penjualanService;
		private readonly IUserSessionService _userSessionService;
		private readonly IPrinterService _printerService;

		[ObservableProperty]
		private string namaBarang = string.Empty;
		[ObservableProperty]
		private ObservableCollection<string> namaBarangList = new();
		[ObservableProperty]
		private ObservableCollection<SatuanItem> itemsSatuan = new();
		[ObservableProperty]
		private ObservableCollection<ProdukSatuanItem> itemsSatuanProduk = new();
		[ObservableProperty]
		private ProdukSatuanItem? selectedSatuanProduk;
		[ObservableProperty]
		private decimal hargaJual;
		[ObservableProperty]
		private int jumlah;
		[ObservableProperty]
		private int sisaStok;
		[ObservableProperty]
		private ObservableCollection<KeranjangItems> itemsKeranjang = new();
		[ObservableProperty]
		private decimal totalHarga;
		[ObservableProperty]
		private decimal pembayaran;
		[ObservableProperty]
		private decimal kembalian;

		private int _stokDasar;

		public PenjualanViewModel(
			IProdukService produkService,
			IProdukSatuanService produkSatuanService,
			ISatuanService satuanService,
			IDialogService dialogService,
			IPenjualanService penjualanService,
			IUserSessionService userSessionService,
			IPrinterService printerService)
		{
			_produkService = produkService;
			_produkSatuanService = produkSatuanService;
			_satuanService = satuanService;
			_dialogService = dialogService;
			_penjualanService = penjualanService;
			_userSessionService = userSessionService;
			_printerService = printerService;

			ItemsKeranjang.CollectionChanged += ItemsKeranjang_CollectionChanged;
			_ = InitializeAsync();
		}

		private void ItemsKeranjang_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (KeranjangItems item in e.NewItems)
				{
					item.PropertyChanged += KeranjangItem_PropertyChanged;
				}
			}

			if (e.OldItems != null)
			{
				foreach (KeranjangItems item in e.OldItems)
				{
					item.PropertyChanged -= KeranjangItem_PropertyChanged;
				}
			}

			HitungTotalHarga();
		}

		private void KeranjangItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(KeranjangItems.SubTotal))
			{
				HitungTotalHarga();
			}
		}

		private async Task InitializeAsync()
		{
			await LoadProdukAsync();
		}

		partial void OnNamaBarangChanged(string value)
		{
			_ = LoadProdukByNamaAsync(value);
		}

		partial void OnSelectedSatuanProdukChanged(ProdukSatuanItem? value)
		{
			if (value == null)
			{
				HargaJual = 0;
				SisaStok = 0;
				return;
			}

			HargaJual = value.HargaJual;

			if (value.IsiPerBox <= 0)
			{
				SisaStok = 0;
				return;
			}

			SisaStok = _stokDasar / value.IsiPerBox;
		}

		partial void OnJumlahChanged(int value)
		{
			if (value > SisaStok)
				Jumlah = SisaStok;
		}

		partial void OnPembayaranChanged(decimal value)
		{
			HitungKembalian();
		}

		private async Task LoadProdukByNamaAsync(string namaBarang)
		{
			if (string.IsNullOrWhiteSpace(namaBarang))
				return;

			var produk = await _produkService.GetByNamaAsync(namaBarang);
			if (produk == null)
				return;

			ItemsSatuanProduk.Clear();

			var produkSatuanList =
				await _produkSatuanService.GetByProdukIdAsync(produk.Id);

			// 🔑 cari satuan dasar (jumlah per satuan terkecil)
			var satuanDasar = produkSatuanList
				.OrderBy(x => x.JumlahPerSatuan)
				.FirstOrDefault();

			// 🔑 total stok dari semua batch
			_stokDasar = satuanDasar?.ProdukBatchs?.Sum(b => b.Stok) ?? 0;

			var satuanDenganHarga =
				produkSatuanList
					.Where(x => x.HargaJual > 0)
					.ToList();

			foreach (var ps in satuanDenganHarga)
			{
				ItemsSatuanProduk.Add(new ProdukSatuanItem
				{
					ProdukId = ps.ProdukId,
					ProdukSatuanId = ps.Id,
					Satuan = new SatuanItem
					{
						Id = ps.Satuan.Id,
						NamaSatuan = ps.Satuan.NamaSatuan
					},
					IsiPerBox = ps.JumlahPerSatuan,
					HargaJual = ps.HargaJual
				});
			}

			SelectedSatuanProduk = ItemsSatuanProduk.FirstOrDefault();
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

		private void HitungTotalHarga()
		{
			TotalHarga = ItemsKeranjang.Sum(x => x.SubTotal);
			HitungKembalian();
		}

		private void HitungKembalian()
		{
			if (Pembayaran <= 0 || Pembayaran < TotalHarga)
			{
				Kembalian = 0;
				return;
			}

			Kembalian = Pembayaran - TotalHarga;
		}

		private void ResetInputSetelahTambah()
		{
			// 🔕 hentikan efek turunan
			Jumlah = 0;

			// reset pilihan satuan
			SelectedSatuanProduk = null;
			ItemsSatuanProduk.Clear();

			// reset produk
			NamaBarang = string.Empty;

			// reset nilai transaksi
			HargaJual = 0;
			SisaStok = 0;
		}

		private void ResetSetelahJual()
		{
			ItemsKeranjang.Clear();

			NamaBarang = string.Empty;
			ItemsSatuanProduk.Clear();
			SelectedSatuanProduk = null;

			Jumlah = 0;
			SisaStok = 0;
			HargaJual = 0;

			TotalHarga = 0;
			Pembayaran = 0;
			Kembalian = 0;
		}

		[RelayCommand]
		private async Task Tambah()
		{
			// 1️⃣ validasi dasar
			if (string.IsNullOrWhiteSpace(NamaBarang))
			{
				await _dialogService.ShowMessage("Nama barang belum diisi.");
				return;
			}

			if (SelectedSatuanProduk == null)
			{
				await _dialogService.ShowMessage("Satuan belum dipilih.");
				return;
			}

			if (Jumlah <= 0)
			{
				await _dialogService.ShowMessage("Jumlah harus lebih dari 0.");
				return;
			}

			if (Jumlah > SisaStok)
			{
				await _dialogService.ShowMessage("Jumlah melebihi sisa stok.");
				Jumlah = SisaStok;
				return;
			}

			// 2️⃣ cek apakah item sudah ada di keranjang
			var existingItem = ItemsKeranjang.FirstOrDefault(x =>
				x.NamaBarang == NamaBarang &&
				x.Satuan?.Id == SelectedSatuanProduk.Satuan?.Id
			);

			if (existingItem != null)
			{
				int totalJumlahBaru = existingItem.Jumlah + Jumlah;

				// ❗ validasi stok gabungan
				if (totalJumlahBaru > SisaStok)
				{
					await _dialogService.ShowMessage(
						"Jumlah total melebihi sisa stok yang tersedia."
					);
					return;
				}
				existingItem.Jumlah += Jumlah;
			}
			else
			{
				// 4️⃣ tambah item baru
				var item = new KeranjangItems
				{
					ProdukId = SelectedSatuanProduk.ProdukId,
					ProdukSatuanId = SelectedSatuanProduk.ProdukSatuanId,
					NamaBarang = NamaBarang,
					Satuan = SelectedSatuanProduk.Satuan,
					HargaJual = HargaJual,
					Jumlah = Jumlah
				};

				ItemsKeranjang.Add(item);
			}

			// 5️⃣ hitung ulang total
			HitungTotalHarga();

			// 6️⃣ reset input setelah tambah
			ResetInputSetelahTambah();
		}

		[RelayCommand]
		private async Task Bersihkan()
		{
			if (!ItemsKeranjang.Any())
			{
				await _dialogService.ShowMessage("Keranjang sudah kosong.");
				return;
			}

			var result = await _dialogService.ShowConfirmation(
				"Konfirmasi",
				"Yakin ingin membersihkan seluruh keranjang?"
			);

			if (result != ContentDialogResult.Primary)
				return;

			// 🔥 bersihkan keranjang
			ItemsKeranjang.Clear();

			// reset transaksi
			TotalHarga = 0;
			Pembayaran = 0;
			Kembalian = 0;
			Jumlah = 0;

			// 🔁 hitung ulang sisa stok (jika satuan masih dipilih)
			if (SelectedSatuanProduk != null && SelectedSatuanProduk.IsiPerBox > 0)
			{
				SisaStok = _stokDasar / SelectedSatuanProduk.IsiPerBox;
			}
			else
			{
				SisaStok = 0;
			}
		}

		[RelayCommand]
		private async Task DeleteProduk(KeranjangItems? item)
		{
			if (item == null)
				return;

			// konfirmasi (opsional tapi disarankan)
			var result = await _dialogService.ShowConfirmation(
				"Konfirmasi",
				$"Hapus {item.NamaBarang} dari keranjang?",
				"Hapus",
				"Batal"
			);

			if (result != ContentDialogResult.Primary)
				return;

			// 1️⃣ hapus dari keranjang
			ItemsKeranjang.Remove(item);

			// 2️⃣ kembalikan stok (jika item yang dihapus sesuai produk aktif)
			if (item.NamaBarang == NamaBarang &&
				SelectedSatuanProduk?.Satuan?.Id == item.Satuan?.Id)
			{
				SisaStok += item.Jumlah;
			}

			// 3️⃣ hitung ulang total
			HitungTotalHarga();
			if (ItemsKeranjang.Count == 0)
			{
				TotalHarga = 0;
				Pembayaran = 0;
				Kembalian = 0;
			}
		}

		public event Action? RequestFocusNamaBarang;

		[RelayCommand]
		private async Task Jual()
		{
			if (!ItemsKeranjang.Any())
			{
				await _dialogService.ShowMessage("Keranjang masih kosong.");
				return;
			}

			if (Pembayaran <= 0)
			{
				await _dialogService.ShowMessage("Masukkan jumlah pembayaran.");
				return;
			}

			if (Pembayaran < TotalHarga)
			{
				await _dialogService.ShowMessage("Pembayaran kurang.");
				return;
			}

			var confirm = await _dialogService.ShowConfirmation(
				"Konfirmasi Pembayaran",
				$"Total: {TotalHarga:N0}\nBayar: {Pembayaran:N0}\nKembalian: {Kembalian:N0}",
				"Proses",
				"Batal"
			);

			if (confirm != ContentDialogResult.Primary)
				return;

			if (!_userSessionService.IsLoggedIn)
			{
				await _dialogService.ShowMessage("User belum login.");
				return;
			}

			try
			{
				var penjualan = await _penjualanService.ProsesPenjualanAsync(
					userId: _userSessionService.CurrentUser!.Id,
					dibayar: Pembayaran,
					keranjang: ItemsKeranjang
				);

				try
				{
					var struk = await _penjualanService.GetStrukAsync(penjualan.Id);

					if (struk != null)
						await _printerService.PrintStrukAsync(struk);
				}
				catch
				{
					// print gagal tidak menggagalkan transaksi
				}

				await _dialogService.ShowMessage("Transaksi berhasil.");

				RequestFocusNamaBarang?.Invoke();

				ResetSetelahJual();
			}
			catch (Exception ex)
			{
				await _dialogService.ShowMessage(ex.Message);
			}
		}
	}
}
