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
		private readonly IRestokService _restokService;
		private readonly IDialogService _dialogService;
		private readonly IPricingService _pricingService;
        private readonly IProdukCacheService _produkCacheService;

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
        private CancellationTokenSource? _cts;
        private bool _isDisposed;
        private bool _isSubscribed;


        public RestokManualViewModel
			(
			IRestokService restokService,
			IPricingService pricingService,
			IDialogService dialogService,
            IProdukCacheService produkCacheService
            )
		{
			_restokService = restokService;
			_pricingService = pricingService;
			_dialogService = dialogService;
            _produkCacheService = produkCacheService;

            _produkCacheService.OnCacheUpdated += OnCacheUpdatedHandler;
        }

        public void Dispose()
        {
            if (!_isSubscribed) return;

            _produkCacheService.OnCacheUpdated -= OnCacheUpdatedHandler;
            _isSubscribed = false;
            _isDisposed = false;
        }

        private void OnCacheUpdatedHandler()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                LoadProduk();
            });
        }

        public async Task EnsureInitializedAsync()
        {
            if (_initialized)
                return;

            _initialized = true;

            if (!_isSubscribed)
            {
                _produkCacheService.OnCacheUpdated += OnCacheUpdatedHandler;
                _isSubscribed = true;
            }

            await _produkCacheService.EnsureLoadedAsync();
            LoadProduk();
        }

        public void RefreshProduk()
        {
            LoadProduk();
        }

        private void LoadProduk()
        {
            NamaBarangList = new ObservableCollection<string>(
				_produkCacheService.GetAll().Select(p => p.NamaBarang)
			);
        }

        partial void OnNamaBarangChanged(string value)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            _ = DebounceAsync(value, token);
        }

        private async Task DebounceAsync(string value, CancellationToken token)
        {
            try
            {
                await Task.Delay(300, token);

                if (token.IsCancellationRequested)
                    return;

                await UpdateItemsSatuanAsync(value);
            }
            catch (TaskCanceledException) { }
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
				.FirstOrDefault();

            if (satuanDasar == null)
                return;

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

            var produk = _produkCacheService.Find(namaBarang);

            if (produk == null)
                return;

            var satuans = await _produkCacheService.GetSatuanAsync(produk.Id);

            foreach (var ps in satuans)
            {
                ItemsSatuan.Add(ps);
            }

            var satuanDasar = ItemsSatuan
				.OrderBy(s => s.JumlahPerSatuan)
				.FirstOrDefault();

            if (satuanDasar == null)
                return;

            SelectedSatuan = ItemsSatuan.FirstOrDefault();
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
