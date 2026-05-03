using MicroCashDevWPFUI.ViewModels.Pages;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace MicroCashDevWPFUI.Views.Pages
{
	public partial class LabaPerhariPage : INavigableView<LabaPerhariViewModel>
	{
		public LabaPerhariViewModel ViewModel { get; }
		public LabaPerhariPage(LabaPerhariViewModel viewModel)
		{
			ViewModel = viewModel;
			DataContext = this;

			InitializeComponent();
		}
	}
}
