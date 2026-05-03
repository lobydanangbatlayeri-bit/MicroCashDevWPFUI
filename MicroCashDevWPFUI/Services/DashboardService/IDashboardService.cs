using MicroCashDevWPFUI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Services.DashboardService
{
	public interface IDashboardService
	{
		Task<decimal> GetTotalPenjualanHariIniAsync();
		Task<decimal> GetLabaHariIniAsync();
		Task<int> GetJumlahNotaBelumBayarAsync();
		Task<int> GetTotalStokAsync();

		Task<List<RecentSaleItem>> GetRecentSalesAsync();
		Task<List<LowStockItem>> GetLowStockProductsAsync();

		Task<(double[] values, string[] labels)> GetSalesChartAsync();
		Task<(double[] values, string[] labels)> GetTopProductChartAsync();
	}
}
