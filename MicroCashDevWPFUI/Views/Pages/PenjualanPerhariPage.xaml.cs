using MicroCashDevWPFUI.ViewModels.Pages;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace MicroCashDevWPFUI.Views.Pages
{
	public partial class PenjualanPerhariPage : INavigableView<PenjualanPerhariViewModel>
	{
		public PenjualanPerhariViewModel ViewModel { get; }
		public PenjualanPerhariPage(PenjualanPerhariViewModel viewModel)
		{
			ViewModel = viewModel;
			DataContext = this;

			InitializeComponent();
		}
	}
}
