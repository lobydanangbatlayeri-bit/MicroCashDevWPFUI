using MicroCashDevWPFUI.ViewModels.Pages;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace MicroCashDevWPFUI.Views.Pages
{
	public partial class TambahNotaPage : INavigableView<TambahNotaViewModel>
	{
		public TambahNotaViewModel ViewModel { get; }
		public TambahNotaPage(TambahNotaViewModel viewModel)
		{
			ViewModel = viewModel;
			DataContext = this;

			InitializeComponent();
		}
	}
}
