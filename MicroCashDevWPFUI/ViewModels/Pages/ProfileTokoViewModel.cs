using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.CoreService;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public partial class ProfileTokoViewModel : ObservableObject
	{
		private readonly IProfileTokoService _service;
		private readonly IDialogService _dialogService;

		private int _id;

		[ObservableProperty]
		private string namaToko = "";

		[ObservableProperty]
		private string alamatToko = "";

		[ObservableProperty]
		private string kataSambutan = "";

		public ProfileTokoViewModel(IProfileTokoService service, IDialogService dialogService)
		{
			_service = service;
			_dialogService = dialogService;

			_ = LoadAsync();
		}

		private async Task LoadAsync()
		{
			var data = await _service.GetAsync();

			if (data == null)
				return;

			_id = data.Id;
			NamaToko = data.NamaToko;
			AlamatToko = data.Alamat;
			KataSambutan = data.KataSambutan;
		}

		[RelayCommand]
		private async Task SimpanAsync()
		{
			if (!await ValidateAsync())
				return;

			var profile = new ProfileToko
			{
				Id = _id,
				NamaToko = NamaToko,
				Alamat = AlamatToko,
				KataSambutan = KataSambutan
			};

			var result = await _service.SaveAsync(profile);
			_id = result.Id;

			await _dialogService.ShowMessage("Profil toko berhasil disimpan.");
		}

		private async Task<bool> ValidateAsync()
		{
			var nama = NamaToko?.Trim() ?? "";
			var alamat = AlamatToko?.Trim() ?? "";
			var sambutan = KataSambutan?.Trim() ?? "";

			if (string.IsNullOrWhiteSpace(nama) &&
				string.IsNullOrWhiteSpace(alamat) &&
				string.IsNullOrWhiteSpace(sambutan))
			{
				await _dialogService.ShowMessage("Semua field masih kosong.");
				return false;
			}

			if (string.IsNullOrWhiteSpace(nama) ||
				string.IsNullOrWhiteSpace(alamat) ||
				string.IsNullOrWhiteSpace(sambutan))
			{
				await _dialogService.ShowMessage("Semua field wajib diisi.");
				return false;
			}

			return true;
		}

	}
}
