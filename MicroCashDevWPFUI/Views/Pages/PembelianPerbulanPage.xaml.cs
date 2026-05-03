using MicroCashDevWPFUI.ViewModels.Pages;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace MicroCashDevWPFUI.Views.Pages
{
	public partial class PembelianPerbulanPage : INavigableView<PembelianPerbulanViewModel>
	{
		public PembelianPerbulanViewModel ViewModel { get; }
		public PembelianPerbulanPage(PembelianPerbulanViewModel viewModel)
		{
			ViewModel = viewModel;
			DataContext = this;

			InitializeComponent();
		}
	}
}
