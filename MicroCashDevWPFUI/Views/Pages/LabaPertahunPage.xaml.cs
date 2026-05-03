using MicroCashDevWPFUI.ViewModels.Pages;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace MicroCashDevWPFUI.Views.Pages
{
	public partial class LabaPertahunPage : INavigableView<LabaPertahunViewModel>
	{
		public LabaPertahunViewModel ViewModel { get; }
		public LabaPertahunPage(LabaPertahunViewModel viewModel)
		{
			ViewModel = viewModel;
			DataContext = this;

			InitializeComponent();
		}
	}
}
