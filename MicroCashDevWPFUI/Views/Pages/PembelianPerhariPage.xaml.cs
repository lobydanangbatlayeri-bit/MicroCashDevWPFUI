using MicroCashDevWPFUI.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace MicroCashDevWPFUI.Views.Pages
{
	public partial class PembelianPerhariPage : INavigableView<PembelianPerhariViewModel>
	{
		public PembelianPerhariViewModel ViewModel { get; }
		public PembelianPerhariPage(PembelianPerhariViewModel viewModel)
		{			
			ViewModel = viewModel;
			DataContext = this;

			InitializeComponent();
		}
	}
}
