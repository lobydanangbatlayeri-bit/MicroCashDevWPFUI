using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Services.DashboardService;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public partial class DashboardViewModel : ObservableObject
	{
		private readonly IDashboardService _dashboardService;

		// ================= SUMMARY =================
		[ObservableProperty] private decimal totalPenjualanHariIni;
		[ObservableProperty] private decimal labaHariIni;
		[ObservableProperty] private int jumlahNotaBelumBayar;
		[ObservableProperty] private int totalStok;

		// ================= CHARTS =================
		[ObservableProperty] private double[] salesValues = Array.Empty<double>();
		[ObservableProperty] private string[] salesLabels = Array.Empty<string>();

		[ObservableProperty] private double[] topProductValues = Array.Empty<double>();
		[ObservableProperty] private string[] topProductLabels = Array.Empty<string>();

		// ================= COLLECTIONS =================
		public ObservableCollection<RecentSaleItem> RecentSales { get; } = new();
		public ObservableCollection<LowStockItem> LowStockProducts { get; } = new();

		// ================= CONSTRUCTOR =================
		public DashboardViewModel(IDashboardService dashboardService)
		{
			_dashboardService = dashboardService;

			// langsung load data async-safe
			_ = LoadDataAsync();
		}

		// ================= LOAD DATA =================
		private async Task LoadDataAsync()
		{
			// --- SUMMARY ---
			TotalPenjualanHariIni = await _dashboardService.GetTotalPenjualanHariIniAsync();
			LabaHariIni = await _dashboardService.GetLabaHariIniAsync();
			JumlahNotaBelumBayar = await _dashboardService.GetJumlahNotaBelumBayarAsync();
			TotalStok = await _dashboardService.GetTotalStokAsync();

			// --- RECENT SALES (maks 10) ---
			RecentSales.Clear();
			var recent = await _dashboardService.GetRecentSalesAsync();
			foreach (var item in recent.Take(10))
				RecentSales.Add(item);

			// --- LOW STOCK (maks 10) ---
			LowStockProducts.Clear();
			var lowStock = await _dashboardService.GetLowStockProductsAsync();
			foreach (var item in lowStock.Take(10))
				LowStockProducts.Add(item);

			// --- CHARTS ---
			(SalesValues, SalesLabels) = await _dashboardService.GetSalesChartAsync();
			(TopProductValues, TopProductLabels) = await _dashboardService.GetTopProductChartAsync();
		}
	}
}