using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.CoreService;
using MicroCashDevWPFUI.Services.PembelianService;
using MicroCashDevWPFUI.Services.ProdukService;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
    public partial class RestokManualViewModel : ObservableObject
    {
		private readonly IProdukService _produkService;
		private readonly IProdukSatuanService _produkSatuanService;
		private readonly IRestokService _restokService;
		private readonly IDialogService _dialogService;
		private readonly IPricingService _pricingService;

		[ObservableProperty] private string namaBarang = string.Empty;
		[ObservableProperty] private ObservableCollection<string> namaBarangList = new();

		[ObservableProperty] private ObservableCollection<ProdukSatuan> itemsSatuan = new();
		[ObservableProperty] private ProdukSatuan? selectedSatuan;

		[ObservableProperty] private DateTime? tanggalKadarluasa;
		[ObservableProperty] private int sisaStok = 0;
		[ObservableProperty] private int tambahStock = 0;
		[ObservableProperty] private decimal hargaJual;
		[ObservableProperty] private string satuanPilih = string.Empty;
		[ObservableProperty] private decimal estimasiHargaBeli;

		private bool _initialized;

		public RestokManualViewModel
			(
			IProdukService produkService,
			IProdukSatuanService produkSatuanService,
			IRestokService restokService,
			IPricingService pricingService,
			IDialogService dialogService
			)
		{
			_produkService = produkService;
			_produkSatuanService = produkSatuanService;
			_restokService = restokService;
			_pricingService = pricingService;
			_dialogService = dialogService;
		}

		public async Task EnsureInitializedAsync()
		{
			if (_initialized)
				return;

			_initialized = true;
			await LoadProdukAsync();
		}

		public async Task RefreshProdukAsync()
		{
			await LoadProdukAsync();
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

		partial void OnNamaBarangChanged(string value)
		{
			_ = UpdateItemsSatuanAsync(value);
		}

		partial void OnSelectedSatuanChanged(ProdukSatuan? value)
		{
			if (value == null)
			{
				HargaJual = 0;
				SatuanPilih = "";
				EstimasiHargaBeli = 0;
				SisaStok = 0;
				return;
			}

			var satuanDasar = ItemsSatuan
				.OrderBy(s => s.JumlahPerSatuan)
				.First();

			int stokDasar = satuanDasar.ProdukBatchs?.Sum(b => b.Stok) ?? 0;

			HargaJual = value.HargaJual;
			SatuanPilih = value.Satuan?.NamaSatuan ?? "";
			EstimasiHargaBeli = _pricingService.GetEstimasiHargaBeli(value);
			SisaStok = stokDasar / value.JumlahPerSatuan;
		}

		private async Task UpdateItemsSatuanAsync(string namaBarang)
		{
			ItemsSatuan.Clear();
			SelectedSatuan = null;			

			if (string.IsNullOrWhiteSpace(namaBarang))
				return;

			var produkList = await _produkService.GetAllAsync();
			var produk = produkList
				.FirstOrDefault(p => p.NamaBarang.Equals(namaBarang, StringComparison.OrdinalIgnoreCase));

			if (produk != null)
			{
				var satuans = await _produkSatuanService.GetByProdukIdAsync(produk.Id);

				foreach (var ps in satuans)
				{
					ItemsSatuan.Add(ps);
				}

				SelectedSatuan = ItemsSatuan.First();
			}
		}

		[RelayCommand]
		private async Task SimpanAsync()
		{
			if (SelectedSatuan == null)
			{
				await _dialogService.ShowMessage("Pilih satuan terlebih dahulu.");
				return;
			}

			if (TambahStock <= 0)
			{
				await _dialogService.ShowMessage("Masukkan jumlah stok yang valid.");
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

			try
			{
				await _restokService.RestokProdukAsync(SelectedSatuan.Id, TambahStock, TanggalKadarluasa ?? DateTime.Now, EstimasiHargaBeli);
				await _dialogService.ShowMessage("Stok berhasil ditambahkan.");

				await UpdateItemsSatuanAsync(NamaBarang);
				ResetInput();
			}
			catch (Exception ex)
			{
				await _dialogService.ShowMessage($"Terjadi error: {ex.Message}");
			}
		}

		[RelayCommand]
		private void Cancel()
		{
			ResetInput();
		}

		private void ResetInput()
		{
			NamaBarang = string.Empty;
			ItemsSatuan.Clear();
			SelectedSatuan = null;
			HargaJual = 0;
			SatuanPilih = string.Empty;
			SisaStok = 0;
			TambahStock = 0;
			TanggalKadarluasa = null;
			EstimasiHargaBeli = 0;
		}

	}
}
