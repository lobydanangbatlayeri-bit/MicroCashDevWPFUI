using MicroCashDevWPFUI.ViewModels.Pages;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace MicroCashDevWPFUI.Views.Pages
{
	public partial class HakAksesPage : INavigableView<HakAksesViewModel>
	{
		public HakAksesViewModel ViewModel { get; }
		public HakAksesPage(HakAksesViewModel viewModel)
		{
			ViewModel = viewModel;
			DataContext = this;

			InitializeComponent();
		}
	}
}
