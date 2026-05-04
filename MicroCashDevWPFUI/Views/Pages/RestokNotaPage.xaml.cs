using MicroCashDevWPFUI.ViewModels.Pages;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Abstractions.Controls;

namespace MicroCashDevWPFUI.Views.Pages
{
	public partial class RestokNotaPage : INavigableView<RestokNotaViewModel>
	{
		public RestokNotaViewModel ViewModel { get; }
		public RestokNotaPage(RestokNotaViewModel viewModel)
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
				cmbSupplier.Focus();
				Keyboard.Focus(cmbSupplier);
			}), System.Windows.Threading.DispatcherPriority.Background);
		}

		private void BtnTambahObat_Click(object sender, RoutedEventArgs e)
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				txtNamaBarang.Focus();
				Keyboard.Focus(txtNamaBarang);
			}), System.Windows.Threading.DispatcherPriority.Background);
		}

		private void BtnSimpan_Click(object sender, RoutedEventArgs e)
		{
			FokusKeSupplier();
		}

		private void BtnBatal_Click(object sender, RoutedEventArgs e)
		{
			FokusKeSupplier();
		}

		private async void BtnTambahSupplier_Click(object sender, RoutedEventArgs e)
		{
			if (ViewModel.BukaTambahSupplierCommand.CanExecute(null))
			{
				await ViewModel.BukaTambahSupplierCommand.ExecuteAsync(null);
			}

			FokusKeSupplier();
		}

		private void FokusKeSupplier()
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				cmbSupplier.Focus();
				Keyboard.Focus(cmbSupplier);
			}), System.Windows.Threading.DispatcherPriority.Background);
		}

		private void RestokManualPage_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.RightShift)
			{
				FokusKeNamaBarang();
				e.Handled = true;
			}
			else if (e.Key == Key.RightCtrl)
			{
				Dispatcher.BeginInvoke(new Action(() =>
				{
					cmbSupplier.Focus();
					Keyboard.Focus(cmbSupplier);
				}), System.Windows.Threading.DispatcherPriority.Background);

				e.Handled = true;
			}
		}

		private void FokusKeNamaBarang()
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				txtNamaBarang.Focus();
				Keyboard.Focus(txtNamaBarang);
			}), System.Windows.Threading.DispatcherPriority.Background);
		}

	}
}
