using MicroCashDevWPFUI.Data;
using MicroCashDevWPFUI.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroCashDevWPFUI.Services.PembelianService
{
	public class DetailPembelianService : IDetailPembelianService
	{
		private readonly IAppDbContext _context;

		public DetailPembelianService(IAppDbContext context)
		{
			_context = context;
		}

		public async Task<List<DetailPembelian>> GetAllAsync()
		{
			return await _context.DetailPembelians
				.IgnoreQueryFilters()
				.Include(dp => dp.ProdukSatuan)
				.ThenInclude(ps => ps.Produk)
				.ToListAsync();
		}

		public async Task<DetailPembelian?> GetByIdAsync(int id)
		{
			return await _context.DetailPembelians
				.IgnoreQueryFilters()
				.Include(dp => dp.ProdukSatuan)
				.ThenInclude(ps => ps.Produk)
				.FirstOrDefaultAsync(dp => dp.Id == id);
		}

		public async Task<List<DetailPembelian>> GetByPembelianIdAsync(int pembelianId)
		{
			return await _context.DetailPembelians
				.IgnoreQueryFilters()
				.Where(dp => dp.PembelianId == pembelianId)
				.Include(dp => dp.ProdukSatuan)
					.ThenInclude(ps => ps.Produk)
				.Include(dp => dp.ProdukSatuan)
					.ThenInclude(ps => ps.Satuan)
				.ToListAsync();
		}

		public async Task<DetailPembelian> AddAsync(DetailPembelian detail)
		{
			_context.DetailPembelians.Add(detail);
			await _context.SaveChangesAsync();
			return detail;
		}

		public async Task AddRangeAsync(IEnumerable<DetailPembelian> details)
		{
			await _context.DetailPembelians.AddRangeAsync(details);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(DetailPembelian detail)
		{
			_context.DetailPembelians.Update(detail);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var entity = await _context.DetailPembelians.FindAsync(id);
			if (entity != null)
			{
				_context.DetailPembelians.Remove(entity);
				await _context.SaveChangesAsync();
			}
		}
	}
}
