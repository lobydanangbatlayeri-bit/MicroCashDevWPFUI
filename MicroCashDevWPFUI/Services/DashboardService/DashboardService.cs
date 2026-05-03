using MicroCashDevWPFUI.Data;
using MicroCashDevWPFUI.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Services.DashboardService
{
	public class DashboardService : IDashboardService
	{
		private readonly IAppDbContext _context;

		public DashboardService(IAppDbContext context)
		{
			_context = context;
		}

		public async Task<decimal> GetTotalPenjualanHariIniAsync()
		{
			var today = DateTime.Today;

			return await _context.Penjualans
				.Where(x => x.Tanggal.Date == today)
				.SumAsync(x => (decimal?)x.Total) ?? 0;
		}

		public async Task<decimal> GetLabaHariIniAsync()
		{
			var today = DateTime.Today;

			var totalPenjualan = await _context.Penjualans
				.Where(p => p.Tanggal.Date == today)
				.SumAsync(p => (decimal?)p.Total) ?? 0;

			var totalHpp = await _context.DetailPenjualanBatches
				.Where(b => b.DetailPenjualan != null
				            && b.DetailPenjualan.Penjualan != null
				            && b.DetailPenjualan.Penjualan.Tanggal.Date == today)
				.SumAsync(b => (decimal?)(b.Jumlah * b.HargaBeli)) ?? 0;

			return totalPenjualan - totalHpp;
		}

		public async Task<int> GetJumlahNotaBelumBayarAsync()
		{
			return await _context.Notas
				.CountAsync(x => !x.StatusPembayaran);
		}

		public async Task<int> GetTotalStokAsync()
		{
			return await _context.ProdukBatchs
				.Where(pb => !pb.ProdukSatuan.Produk.IsDeleted)
				.SumAsync(x => x.Stok);
		}

		public async Task<List<RecentSaleItem>> GetRecentSalesAsync()
		{
			return await _context.Penjualans
				.OrderByDescending(x => x.Tanggal)
				.Take(5)
				.Select(x => new RecentSaleItem
				{
					NomorNota = x.NomorNota,
					Tanggal = x.Tanggal,
					Total = x.Total
				})
				.ToListAsync();
		}

		public async Task<List<LowStockItem>> GetLowStockProductsAsync()
		{
			var data = await _context.Produks
				.Select(p => new
				{
					Nama = p.NamaBarang,
					TotalStok = p.ProdukSatuans
						.SelectMany(ps => ps.ProdukBatchs)
						.Sum(pb => (int?)pb.Stok) ?? 0
				})
				.Where(x => x.TotalStok <= 5)
				.ToListAsync();

			return data.Select(x => new LowStockItem
			{
				NamaProduk = x.Nama,
				Stok = x.TotalStok
			}).ToList();
		}

		public async Task<(double[], string[])> GetSalesChartAsync()
		{
			var startDate = DateTime.Today.AddDays(-6);

			var data = await _context.Penjualans
				.Where(x => x.Tanggal.Date >= startDate)
				.GroupBy(x => x.Tanggal.Date)
				.Select(g => new
				{
					Date = g.Key,
					Total = g.Sum(x => x.Total)
				})
				.ToListAsync();

			var last7Days = Enumerable.Range(0, 7)
				.Select(i => DateTime.Today.AddDays(-i))
				.Reverse()
				.ToList();

			var values = last7Days
				.Select(date => (double)(data.FirstOrDefault(x => x.Date == date)?.Total ?? 0))
				.ToArray();

			var labels = last7Days
				.Select(d => d.ToString("dd MMM"))
				.ToArray();

			return (values, labels);
		}

		public async Task<(double[], string[])> GetTopProductChartAsync()
		{
			var data = await _context.DetailPenjualans
				.GroupBy(x => x.ProdukSatuan.Produk.NamaBarang)
				.Select(g => new
				{
					Nama = g.Key,
					Total = g.Sum(x => x.Jumlah)
				})
				.OrderByDescending(x => x.Total)
				.Take(5)
				.ToListAsync();

			return (
				data.Select(x => (double)x.Total).ToArray(),
				data.Select(x => x.Nama).ToArray()
			);
		}
	}
}
