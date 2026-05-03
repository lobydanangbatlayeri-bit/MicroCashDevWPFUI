using MicroCashDevWPFUI.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace MicroCashDevWPFUI.Views.Pages
{
	public partial class PembelianPertahunPage : INavigableView<PembelianPertahunViewModel>
	{
		public PembelianPertahunViewModel ViewModel { get; }
		public PembelianPertahunPage(PembelianPertahunViewModel viewModel)
		{
			ViewModel = viewModel;
			DataContext = this;

			InitializeComponent();
		}
	}
}
