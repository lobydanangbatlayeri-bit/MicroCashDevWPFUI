using MicroCashDevWPFUI.Data;
using MicroCashDevWPFUI.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroCashDevWPFUI.Services.ProdukService
{
	public class SatuanService : ISatuanService
	{
		private readonly IAppDbContext _context;

		public SatuanService(IAppDbContext context)
		{
			_context = (AppDbContext)context;
		}

		public async Task<IEnumerable<Satuan>> GetAllAsync()
		{
			return await _context.Satuans
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<Satuan?> GetByIdAsync(int id)
		{
			return await _context.Satuans
				.FirstOrDefaultAsync(s => s.Id == id);
		}

		public async Task<Satuan> AddAsync(Satuan satuan)
		{
			_context.Satuans.Add(satuan);
			await _context.SaveChangesAsync();
			return satuan;
		}

		public async Task<bool> UpdateAsync(Satuan satuan)
		{
			var existing = await _context.Satuans.FindAsync(satuan.Id);
			if (existing == null) return false;

			existing.NamaSatuan = satuan.NamaSatuan;
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var satuan = await GetByIdAsync(id);
			if (satuan == null)
				return false;

			// ambil semua ProdukSatuan yang pakai Satuan ini
			var produkSatuans = await _context.ProdukSatuans
				.Where(p => p.SatuanId == id)
				.ToListAsync();

			foreach (var ps in produkSatuans)
			{
				// cek apakah masih ada DetailPenjualan terkait
				bool usedInPenjualan = await _context.Set<DetailPenjualan>()
					.AnyAsync(d => d.ProdukSatuanId == ps.Id);
				if (usedInPenjualan)
					throw new InvalidOperationException($"Tidak bisa menghapus Satuan karena ProdukSatuan '{ps.Id}' masih digunakan di penjualan.");

				// cek apakah masih ada DetailPembelian terkait
				bool usedInPembelian = await _context.Set<DetailPembelian>()
					.AnyAsync(d => d.ProdukSatuanId == ps.Id);
				if (usedInPembelian)
					throw new InvalidOperationException($"Tidak bisa menghapus Satuan karena ProdukSatuan '{ps.Id}' masih digunakan di pembelian.");

				// cek apakah masih ada ProdukBatch terkait
				bool usedInBatch = await _context.Set<ProdukBatch>()
					.AnyAsync(d => d.ProdukSatuanId == ps.Id);
				if (usedInBatch)
					throw new InvalidOperationException($"Tidak bisa menghapus Satuan karena ProdukSatuan '{ps.Id}' masih digunakan di batch produk.");
			}

			// jika aman, hapus semua ProdukSatuan yang terkait
			if (produkSatuans.Any())
				_context.ProdukSatuans.RemoveRange(produkSatuans);

			// hapus Satuan
			_context.Satuans.Remove(satuan);

			return await _context.SaveChangesAsync() > 0;
		}

	}
}
