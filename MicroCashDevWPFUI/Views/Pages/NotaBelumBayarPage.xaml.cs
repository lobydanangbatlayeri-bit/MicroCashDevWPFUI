using MicroCashDevWPFUI.ViewModels.Pages;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace MicroCashDevWPFUI.Views.Pages
{
	public partial class NotaBelumBayarPage : INavigableView<NotaBelumBayarViewModel>
	{
		public NotaBelumBayarViewModel ViewModel { get; }
		public NotaBelumBayarPage(NotaBelumBayarViewModel viewModel)
		{
			ViewModel = viewModel;
			DataContext = this;

			InitializeComponent();
		}
	}
}
