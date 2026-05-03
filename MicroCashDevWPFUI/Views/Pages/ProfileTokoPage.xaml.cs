using MicroCashDevWPFUI.ViewModels.Pages;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace MicroCashDevWPFUI.Views.Pages
{
	public partial class ProfileTokoPage : INavigableView<ProfileTokoViewModel>
	{
		public ProfileTokoViewModel ViewModel { get; }
		public ProfileTokoPage(ProfileTokoViewModel viewModel)
		{
			ViewModel = viewModel;
			DataContext = this;

			InitializeComponent();
		}
	}
}
