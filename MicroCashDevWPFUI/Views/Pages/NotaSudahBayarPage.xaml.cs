using MicroCashDevWPFUI.ViewModels.Pages;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace MicroCashDevWPFUI.Views.Pages
{
	public partial class NotaSudahBayarPage : INavigableView<NotaSudahBayarViewModel>
	{
		public NotaSudahBayarViewModel ViewModel { get; }
		public NotaSudahBayarPage(NotaSudahBayarViewModel viewModel)
		{
			ViewModel = viewModel;
			DataContext = this;

			InitializeComponent();
		}
	}
}
