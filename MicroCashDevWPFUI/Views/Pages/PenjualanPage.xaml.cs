using MicroCashDevWPFUI.ViewModels.Pages;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Abstractions.Controls;

namespace MicroCashDevWPFUI.Views.Pages
{
	
	public partial class PenjualanPage : INavigableView<PenjualanViewModel>
	{
		public PenjualanViewModel ViewModel { get; }
		public PenjualanPage(PenjualanViewModel viewModel)
		{
			ViewModel = viewModel;
			DataContext = this;

			InitializeComponent();

			this.PreviewKeyDown += PenjualanPage_PreviewKeyDown;
			ViewModel.RequestFocusNamaBarang += FokusKeNamaBarang;
		}

		private void PenjualanPage_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.RightCtrl)
			{
				Dispatcher.BeginInvoke(new Action(() =>
				{
					txtPembayaran.Focus();
					Keyboard.Focus(txtPembayaran);

					// Letakkan caret di akhir
					txtPembayaran.CaretIndex = txtPembayaran.Text.Length;
				}), System.Windows.Threading.DispatcherPriority.Background);

				e.Handled = true; // mencegah event ditangani lagi
			}
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				txtNamaBarang.Focus();
				Keyboard.Focus(txtNamaBarang);
			}), System.Windows.Threading.DispatcherPriority.Background);
		}

		private void FokusKeNamaBarang()
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				txtNamaBarang.Focus();
				Keyboard.Focus(txtNamaBarang);
			}), System.Windows.Threading.DispatcherPriority.Background);
		}

		private void BtnTambah_Click(object sender, RoutedEventArgs e)
		{
			FokusKeNamaBarang();
		}

		private void BtnBersihkan_Click(object sender, RoutedEventArgs e)
		{
			FokusKeNamaBarang();
		}

		private void BtnJual_Click(object sender, RoutedEventArgs e)
		{
			FokusKeNamaBarang();
		}

		private void BtnUangPas_Click(object sender, RoutedEventArgs e)
		{
			if (ViewModel.TotalHarga > 0)
			{
				ViewModel.Pembayaran = ViewModel.TotalHarga;
				txtPembayaran.Focus();
				txtPembayaran.CaretIndex = txtPembayaran.Text.Length;
			}
		}

	}
}
