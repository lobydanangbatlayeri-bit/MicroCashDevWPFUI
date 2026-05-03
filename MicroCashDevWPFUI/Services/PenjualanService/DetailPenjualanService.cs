using MicroCashDevWPFUI.Data;
using MicroCashDevWPFUI.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroCashDevWPFUI.Services.PenjualanService
{
	public class DetailPenjualanService : IDetailPenjualanService
	{
		private readonly IAppDbContext _context;

		public DetailPenjualanService(IAppDbContext context)
		{
			_context = context;
		}

		public async Task<List<DetailPenjualan>> GetAllAsync()
		{
			return await _context.DetailPenjualans
				.IgnoreQueryFilters()
				.Include(dp => dp.ProdukSatuan)
					.ThenInclude(ps => ps.Produk)
				.ToListAsync();
		}

		public async Task<DetailPenjualan?> GetByIdAsync(int id)
		{
			return await _context.DetailPenjualans
				.IgnoreQueryFilters()
				.Include(dp => dp.ProdukSatuan)
					.ThenInclude(ps => ps.Produk)
				.FirstOrDefaultAsync(dp => dp.Id == id);
		}

		public async Task<List<DetailPenjualan>> GetByPenjualanIdAsync(int penjualanId)
		{
			return await _context.DetailPenjualans
				.IgnoreQueryFilters()
				.Where(dp => dp.PenjualanId == penjualanId)
				.Include(dp => dp.ProdukSatuan)
					.ThenInclude(ps => ps.Produk)
				.Include(dp => dp.ProdukSatuan)
					.ThenInclude(ps => ps.Satuan)
				.ToListAsync();
		}

		public async Task<DetailPenjualan> AddAsync(DetailPenjualan detail)
		{
			_context.DetailPenjualans.Add(detail);
			await _context.SaveChangesAsync();
			return detail;
		}

		public async Task AddRangeAsync(IEnumerable<DetailPenjualan> details)
		{
			await _context.DetailPenjualans.AddRangeAsync(details);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(DetailPenjualan detail)
		{
			_context.DetailPenjualans.Update(detail);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var entity = await _context.DetailPenjualans.FindAsync(id);
			if (entity != null)
			{
				_context.DetailPenjualans.Remove(entity);
				await _context.SaveChangesAsync();
			}
		}
	}
}
