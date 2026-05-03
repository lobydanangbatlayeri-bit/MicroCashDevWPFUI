using MicroCashDevWPFUI.Data;
using MicroCashDevWPFUI.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroCashDevWPFUI.Services.ProdukService
{
	public class ProdukService : IProdukService
	{
		private readonly IAppDbContext _context;

		public ProdukService(IAppDbContext context)
		{
			_context = context;
		}

		public async Task<List<Produk>> GetAllAsync()
		{
			return await _context.Produks
				.Include(p => p.ProdukSatuans)
				.ToListAsync();
		}

		public async Task<Produk?> GetByIdAsync(int id)
		{
			return await _context.Produks
				.Include(p => p.ProdukSatuans)
				.FirstOrDefaultAsync(p => p.Id == id);
		}

		public async Task<Produk?> GetByNamaAsync(string namaBarang)
		{
			return await _context.Produks
				.Include(p => p.ProdukSatuans)
					.ThenInclude(ps => ps.Satuan)
				.FirstOrDefaultAsync(p => p.NamaBarang == namaBarang);
		}


		public async Task<Produk> AddAsync(Produk produk)
		{
			_context.Produks.Add(produk);
			await _context.SaveChangesAsync();
			return produk;
		}

		public async Task UpdateAsync(Produk produk)
		{
			_context.Produks.Update(produk);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var produk = await _context.Produks.FindAsync(id);
			if (produk == null)
				return;

			produk.IsDeleted = true;
			produk.DeletedAt = DateTime.Now;

			_context.Produks.Update(produk);
			await _context.SaveChangesAsync();
		}
	}
}
