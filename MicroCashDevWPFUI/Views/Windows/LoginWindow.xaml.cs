using CommunityToolkit.Mvvm.Messaging;
using MicroCashDevWPFUI.Messages;
using MicroCashDevWPFUI.ViewModels.Windows;
using System.Windows.Input;
using Wpf.Ui;

namespace MicroCashDevWPFUI.Views.Windows
{
	public partial class LoginWindow : Window
	{
		private readonly IContentDialogService _dialogService;
		public LoginWindow(LoginWindowViewModel vm, INavigationWindow navigationWindow, IContentDialogService dialogService)
		{
			InitializeComponent();
			DataContext = vm;

			_dialogService = dialogService;

			WeakReferenceMessenger.Default.Register<LoginSuccessMessage>(this, (_, message) =>
			{
				var user = message;

				navigationWindow.ShowWindow();
				navigationWindow.Navigate(typeof(Views.Pages.DashboardPage));

				this.Hide();
			});
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			_dialogService.SetDialogHost(LoginDialogHost);
		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Keyboard.ClearFocus();
				DragMove();
			}
		}

		private void BtnMinimize_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void BtnClose_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void UsernameTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				PasswordBox.Focus();
			}
		}
	}
}
