using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Data;
using Microsoft.EntityFrameworkCore;

namespace MicroCashDevWPFUI.Services.ProdukService
{
	public class ProdukSatuanService : IProdukSatuanService
	{
		private readonly IAppDbContext _context;

		public ProdukSatuanService(IAppDbContext context)
		{
			_context = context;
		}

		public async Task<List<ProdukSatuan>> GetAllAsync()
		{
			return await _context.ProdukSatuans
				.Include(ps => ps.Produk)
				.Include(ps => ps.Satuan)
				.Include(ps => ps.ProdukBatchs)
				.ToListAsync();
		}

		public async Task<ProdukSatuan?> GetByIdAsync(int id)
		{
			return await _context.ProdukSatuans
				.Include(ps => ps.Produk)
				.Include(ps => ps.Satuan)
				.Include(ps => ps.ProdukBatchs)   // <-- tambahkan include
				.FirstOrDefaultAsync(ps => ps.Id == id);
		}

		public async Task<List<ProdukSatuan>> GetByProdukIdAsync(int produkId)
		{
			return await _context.ProdukSatuans
				.Include(ps => ps.Produk)
				.Include(ps => ps.Satuan)
				.Include(ps => ps.ProdukBatchs)
				.Where(ps => ps.ProdukId == produkId)
				.ToListAsync();
		}

		public async Task AddAsync(ProdukSatuan produkSatuan)
		{
			_context.ProdukSatuans.Add(produkSatuan);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(ProdukSatuan produkSatuan)
		{
			_context.ProdukSatuans.Update(produkSatuan);
			await _context.SaveChangesAsync();
		}
		public async Task DeleteAsync(int id)
		{
			var produkSatuan = await _context.ProdukSatuans
				.FirstOrDefaultAsync(ps => ps.Id == id);

			if (produkSatuan == null)
				return;

			// 🔍 CEK DETAIL PENJUALAN
			bool usedInPenjualan = await _context.Set<DetailPenjualan>()
				.AnyAsync(d => d.ProdukSatuanId == id);

			if (usedInPenjualan)
				throw new InvalidOperationException(
					"Tidak bisa menghapus satuan karena sudah digunakan dalam transaksi penjualan."
				);

			// 🔍 CEK DETAIL PEMBELIAN
			bool usedInPembelian = await _context.Set<DetailPembelian>()
				.AnyAsync(d => d.ProdukSatuanId == id);

			if (usedInPembelian)
				throw new InvalidOperationException(
					"Tidak bisa menghapus satuan karena sudah digunakan dalam transaksi pembelian."
				);

			// 🔍 CEK PRODUK BATCH
			bool usedInBatch = await _context.Set<ProdukBatch>()
				.AnyAsync(b => b.ProdukSatuanId == id);

			if (usedInBatch)
				throw new InvalidOperationException(
					"Tidak bisa menghapus satuan karena sudah memiliki riwayat restok."
				);

			_context.ProdukSatuans.Remove(produkSatuan);
			await _context.SaveChangesAsync();
		}

		public async Task AddRangeAsync(IEnumerable<ProdukSatuan> produkSatuanList)
		{
			await _context.ProdukSatuans.AddRangeAsync(produkSatuanList);
			await _context.SaveChangesAsync();
		}

	}
}