using MicroCashDevWPFUI.ViewModels.Pages;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace MicroCashDevWPFUI.Views.Pages
{
	public partial class PenjualanPertahunPage : INavigableView<PenjualanPertahunViewModel>
	{
		public PenjualanPertahunViewModel ViewModel { get; }
		public PenjualanPertahunPage(PenjualanPertahunViewModel viewModel)
		{
			ViewModel = viewModel;
			DataContext = this;

			InitializeComponent();
		}
	}
}
