using MicroCashDevWPFUI.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Abstractions.Controls;

namespace MicroCashDevWPFUI.Views.Pages
{
	public partial class LabaPerbulanPage : INavigableView<LabaPerbulanViewModel>
	{
		public LabaPerbulanViewModel ViewModel { get; }
		public LabaPerbulanPage(LabaPerbulanViewModel viewModel)
		{
			ViewModel = viewModel;
			DataContext = this;

			InitializeComponent();
		}
	}
}
