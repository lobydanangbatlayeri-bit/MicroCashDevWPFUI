using CommunityToolkit.Mvvm.Messaging;
using MicroCashDevWPFUI.Messages;
using MicroCashDevWPFUI.Services.CoreService;
using MicroCashDevWPFUI.Views.Windows;
using System.Windows.Navigation;
using Wpf.Ui;

namespace MicroCashDevWPFUI.ViewModels.Windows
{
	public partial class LoginWindowViewModel : ObservableObject
	{
		private readonly IUserService _userService;
		private readonly IDialogService _dialogService;
		private readonly IUserSessionService _session;

		public LoginWindowViewModel(
			IUserService userService,
			IDialogService dialogService,
			INavigationWindow navigationWindow,
			IUserSessionService session
		)
		{
			_userService = userService;
			_dialogService = dialogService;
			_session = session;
		}

		[ObservableProperty]
		private string username = "";

		[ObservableProperty]
		private string password = "";

		[RelayCommand]
		private async Task Login()
		{
			if (string.IsNullOrWhiteSpace(Username))
			{
				await _dialogService.ShowMessage("Username tidak boleh kosong.");
				return;
			}

			if (string.IsNullOrWhiteSpace(Password))
			{
				await _dialogService.ShowMessage("Password tidak boleh kosong.");
				return;
			}

			var user = await _userService.LoginAsync(Username, Password);

			if (user == null)
			{
				await _dialogService.ShowMessage("Gagal login. Periksa kembali username dan password Anda.");
				return;
			}

			_session.CurrentUser = user;
			WeakReferenceMessenger.Default.Send(new LoginSuccessMessage(user));
			Clear();
		}

		private void Clear()
		{
			Username = string.Empty;
			Password = string.Empty;
		}
	}
}
