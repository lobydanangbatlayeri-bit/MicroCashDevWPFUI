using MicroCashDevWPFUI.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace MicroCashDevWPFUI.Views.Pages
{	
	public partial class ProdukPage : INavigableView<ProdukViewModel>
	{
		public ProdukViewModel ViewModel { get; }
		public ProdukPage(ProdukViewModel viewModel)
		{
			ViewModel = viewModel;
			DataContext = this;

			InitializeComponent();
		}
	}
}
