using MicroCashDevWPFUI.ViewModels.Pages;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace MicroCashDevWPFUI.Views.Pages
{
	public partial class ScanNotaPage : INavigableView<ScanNotaViewModel>
	{
		public ScanNotaViewModel ViewModel { get; }
		public ScanNotaPage(ScanNotaViewModel viewModel)
		{
			ViewModel = viewModel;
			DataContext = this;

			InitializeComponent();
		}
	}
}
