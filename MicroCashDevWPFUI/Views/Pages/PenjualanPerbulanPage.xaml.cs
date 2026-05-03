using MicroCashDevWPFUI.ViewModels.Pages;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace MicroCashDevWPFUI.Views.Pages
{
	public partial class PenjualanPerbulanPage : INavigableView<PenjualanPerbulanViewModel>
	{
		public PenjualanPerbulanViewModel ViewModel { get; }
		public PenjualanPerbulanPage(PenjualanPerbulanViewModel viewModel)
		{
			ViewModel = viewModel;
			DataContext = this;

			InitializeComponent();
		}
	}
}
