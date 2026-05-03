using MicroCashDevWPFUI.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace MicroCashDevWPFUI.Views.Pages
{
    public partial class DashboardPage : INavigableView<DashboardViewModel>
    {
        public DashboardViewModel ViewModel { get; }

        public DashboardPage(DashboardViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();

			Loaded += DashboardPage_Loaded;
		}

		private void DashboardPage_Loaded(object sender, RoutedEventArgs e)
		{
			SetupSalesChart();
			SetupTopProductChart();
		}

		private void SetupSalesChart()
		{
			SalesPlot.Plot.Clear();

			double[] values = ViewModel.SalesValues;
			double[] positions = Enumerable.Range(0, values.Length)
										   .Select(x => (double)x)
										   .ToArray();

			var scatter = SalesPlot.Plot.Add.Scatter(positions, values);
			scatter.LineWidth = 3;

			SalesPlot.Plot.Axes.Bottom.TickGenerator =
				new ScottPlot.TickGenerators.NumericManual(
					positions,
					ViewModel.SalesLabels
				);

			SalesPlot.Plot.Axes.Left.Label.Text = "Total Penjualan (Rp)";
			SalesPlot.Plot.Axes.Bottom.Label.Text = "Periode";

			SalesPlot.Plot.Axes.AutoScale();
			SalesPlot.Refresh();
		}

		private void SetupTopProductChart()
		{
			TopProductPlot.Plot.Clear();

			double[] values = ViewModel.TopProductValues;
			double[] positions = Enumerable.Range(0, values.Length)
										   .Select(x => x * 2.0)
										   .ToArray();

			var bar = TopProductPlot.Plot.Add.Bars(positions, values);

			TopProductPlot.Plot.Axes.Bottom.TickGenerator =
				new ScottPlot.TickGenerators.NumericManual(
					positions,
					ViewModel.TopProductLabels
				);

			TopProductPlot.Plot.Axes.Left.Label.Text = "Jumlah Terjual (Unit)";
			TopProductPlot.Plot.Axes.Bottom.Label.Text = "Produk";
			TopProductPlot.Plot.Axes.Bottom.TickLabelStyle.Rotation = 45;

			TopProductPlot.Plot.Axes.AutoScale();
			TopProductPlot.Refresh();
		}
	}
}
