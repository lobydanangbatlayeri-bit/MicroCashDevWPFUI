using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.CoreService;
using MicroCashDevWPFUI.Services.KeuanganService;
using MicroCashDevWPFUI.Services.PembelianService;
using MicroCashDevWPFUI.Services.PenjualanService;
using MicroCashDevWPFUI.Views.Controls;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public partial class TambahNotaViewModel : ObservableObject
	{
		private readonly ISupplierService _supplierService;
		private readonly IPembelianService _pembelianService;
		private readonly IDialogService _dialogService;
		private readonly IBankService _bankService;
		private readonly INotaService _notaService;

		[ObservableProperty]
		private ObservableCollection<SupplierItem> itemsSupplier = new();

		[ObservableProperty]
		private SupplierItem? selectedSupplier;

		[ObservableProperty]
		private string nomorFaktur = string.Empty;

		[ObservableProperty]
		private ObservableCollection<string> nomorFakturList = new();

		[ObservableProperty]
		private bool statusPembayaran;

		[ObservableProperty]
		private DateTime? tanggalMasuk;

		[ObservableProperty]
		private DateTime? tanggalDeadline;

		[ObservableProperty]
		private ObservableCollection<BankItem> itemsBank = new();

		[ObservableProperty]
		private decimal total;

		[ObservableProperty]
		private string namaBankBaru = string.Empty;

		[ObservableProperty]
		private string nomorRekeningBaru = string.Empty;

		[ObservableProperty]
		private int? pembelianId;

		public TambahNotaViewModel(ISupplierService supplierService, IPembelianService pembelianService, IDialogService dialogService, IBankService bankService, INotaService notaService)
		{
			_supplierService = supplierService;
			_pembelianService = pembelianService;
			_dialogService = dialogService;
			_bankService = bankService;
			_notaService = notaService;

			_ = InitializeAsync();
		}

		public async Task InitializeAsync()
		{
			await LoadSupplierAsync();
		}

		partial void OnSelectedSupplierChanged(SupplierItem? value)
		{
			// Reset semua state turunan
			NomorFaktur = string.Empty;
			NomorFakturList.Clear();
			TanggalDeadline = null;
			TanggalMasuk = null;
			Total = 0;

			ItemsBank.Clear();

			if (value is null)
				return;

			_ = LoadNomorFakturAsync(value.Id);
			_ = LoadBankAsync(value.Id);
		}

		partial void OnNomorFakturChanged(string value)
		{
			TanggalMasuk = null;
			Total = 0;

			if (string.IsNullOrWhiteSpace(value))
				return;

			if (!NomorFakturList.Contains(value))
				return;

			_ = LoadPembelianDetailAsync(value);
		}

		public async Task LoadSupplierAsync()
		{
			var suppliers = await _supplierService.GetAllAsync();

			ItemsSupplier = new ObservableCollection<SupplierItem>(
				suppliers.Select((s, index) => new SupplierItem
				{
					Nomor = index + 1,
					Id = s.Id,
					NamaSupplier = s.NamaSupplier,
					Alamat = s.Alamat
				}));
		}

		private async Task LoadNomorFakturAsync(int supplierId)
		{
			var fakturList = await _pembelianService
				.GetNomorFakturBySupplierAsync(supplierId);

			NomorFakturList = new ObservableCollection<string>(fakturList);
		}

		private async Task LoadPembelianDetailAsync(string nomorFaktur)
		{
			var pembelian = await _pembelianService
				.GetByNomorFakturAsync(nomorFaktur);

			if (pembelian == null)
				return;

			PembelianId = pembelian.Id;
			TanggalMasuk = pembelian.Tanggal;
			Total = pembelian.Total;
		}

		private async Task LoadBankAsync(int supplierId)
		{
			var banks = await _bankService.GetBySupplierIdAsync(supplierId);

			ItemsBank = new ObservableCollection<BankItem>(
				banks.Select((b, index) => new BankItem
				{
					Nomor = index + 1,
					Id = b.Id,
					SupplierId = b.SupplierId,
					NamaBank = b.NamaBank,
					NomorRekening = b.NomorRekening
				}));
		}

		private async Task SimpanBank()
		{
			if (SelectedSupplier == null)
				return;

			if (string.IsNullOrWhiteSpace(NamaBankBaru) ||
				string.IsNullOrWhiteSpace(NomorRekeningBaru))
				return;

			var bank = new Bank
			{
				SupplierId = SelectedSupplier.Id,
				NamaBank = NamaBankBaru,
				NomorRekening = NomorRekeningBaru
			};

			var saved = await _bankService.AddAsync(bank);

			// Tambahkan ke ItemsBank (DTO)
			ItemsBank.Add(new BankItem
			{
				Id = saved.Id,
				SupplierId = saved.SupplierId,
				NamaBank = saved.NamaBank,
				NomorRekening = saved.NomorRekening,
				Nomor = ItemsBank.Count + 1
			});
		}

		[RelayCommand]
		async Task DeleteBankAsync(BankItem? item)
		{
			if (item == null) return;

			var result = await _dialogService.ShowConfirmation(
				"Hapus Bank",
				"Apakah Anda yakin ingin menghapus bank ini?",
				"Hapus",
				"Batal");

			// Hanya hapus kalau user pilih tombol utama (Hapus)
			if (result != ContentDialogResult.Primary) return;

			// Hapus dari database
			await _bankService.DeleteAsync(item.Id);

			// Hapus dari ObservableCollection
			ItemsBank.Remove(item);
		}

		[RelayCommand]
		private async Task BukaTambahBank()
		{
			var view = new TambahBankDialog()
			{
				DataContext = this
			};

			var result = await _dialogService.ShowContentDialogAsync(
				"Tambah Bank",
				view,
				"Simpan",
				"Batal");

			if (result == ContentDialogResult.Primary)
				await SimpanBank();

			NamaBankBaru = string.Empty;
			NomorRekeningBaru = string.Empty;
		}

		[RelayCommand]
		private async Task Simpan()
		{
			if (SelectedSupplier == null)
			{
				await _dialogService.ShowMessage("Pilih supplier terlebih dahulu.");
				return;
			}

			if (string.IsNullOrWhiteSpace(NomorFaktur))
			{
				await _dialogService.ShowMessage("Pilih nomor faktur yang valid.");
				return;
			}

			var pembelian = await _pembelianService.GetByNomorFakturAsync(NomorFaktur);
			if (pembelian == null)
			{
				await _dialogService.ShowMessage("Nomor faktur tidak ditemukan.");
				return;
			}

			if (await _notaService.ExistsByPembelianIdAsync(pembelian.Id))
			{
				await _dialogService.ShowMessage("Nota untuk pembelian ini sudah ada.");
				return;
			}

			if (TanggalDeadline == null)
			{
				await _dialogService.ShowMessage("Tanggal deadline harus diisi.");
				return;
			}

			if (Total <= 0)
			{
				await _dialogService.ShowMessage("Total harus lebih dari 0.");
				return;
			}

			var nota = new Nota
			{
				PembelianId = pembelian.Id,
				StatusPembayaran = StatusPembayaran,
				TanggalDeadline = TanggalDeadline ?? DateTime.Now,
				Total = Total
			};

			await _notaService.CreateAsync(nota);

			await _dialogService.ShowMessage("Nota berhasil disimpan.");

			Cancel();
		}

		[RelayCommand]
		private void Cancel()
		{
			SelectedSupplier = null;
			NomorFaktur = string.Empty;
			NomorFakturList.Clear();

			TanggalMasuk = null;
			TanggalDeadline = null;
			Total = 0;
			StatusPembayaran = false;

			ItemsBank.Clear();
		}

	}
}
