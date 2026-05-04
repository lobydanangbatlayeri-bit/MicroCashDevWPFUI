using MicroCashDevWPFUI.ViewModels.Pages;
using System.Windows.Input;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;

namespace MicroCashDevWPFUI.Views.Pages
{
	public partial class RestokManualPage : INavigableView<RestokManualViewModel>
	{
		public RestokManualViewModel ViewModel { get; }
		public RestokManualPage(RestokManualViewModel viewModel)
		{
			ViewModel = viewModel;
			DataContext = this;

			InitializeComponent();

            Loaded += OnLoaded;

            Unloaded += OnUnloaded;

            this.PreviewKeyDown += RestokManualPage_PreviewKeyDown;
		}

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.EnsureInitializedAsync();
            Loaded -= OnLoaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Dispose();
            Unloaded -= OnUnloaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				txtNamaBarang.Focus();
				Keyboard.Focus(txtNamaBarang);
			}), System.Windows.Threading.DispatcherPriority.Background);
		}

		private void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			FokusKeNamaBarang();
		}

		private void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			FokusKeNamaBarang();
		}

		private void FokusKeNamaBarang()
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				txtNamaBarang.Focus();
				Keyboard.Focus(txtNamaBarang);
			}), System.Windows.Threading.DispatcherPriority.Background);
		}

		private void RestokManualPage_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.RightShift)
			{
				FokusKeNamaBarang();
				e.Handled = true;
			}
		}
	}
}
