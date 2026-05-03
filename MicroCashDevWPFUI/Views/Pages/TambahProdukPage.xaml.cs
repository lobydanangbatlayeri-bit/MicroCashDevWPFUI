using MicroCashDevWPFUI.ViewModels.Pages;
using System.Windows.Input;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;

namespace MicroCashDevWPFUI.Views.Pages
{
	public partial class TambahProdukPage : INavigableView<TambahProdukViewModel>
	{
		public TambahProdukViewModel ViewModel { get; }
		public TambahProdukPage(TambahProdukViewModel viewModel)
		{
			ViewModel = viewModel;
			DataContext = this;

			InitializeComponent();

			this.PreviewKeyDown += TambahProdukPage_PreviewKeyDown;
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			txtNamaBarang.Focus();
			Keyboard.Focus(txtNamaBarang);
		}

		private void BtnTambah_Click(object sender, RoutedEventArgs e)
		{
			// Tunggu UI selesai update setelah command
			Dispatcher.BeginInvoke(new Action(() =>
			{
				cmbPilihSatuan.Focus();
				Keyboard.Focus(cmbPilihSatuan);
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

		private void TambahProdukPage_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.RightShift)
			{
				FokusKeNamaBarang();
				e.Handled = true;
			}
		}

	}
}
