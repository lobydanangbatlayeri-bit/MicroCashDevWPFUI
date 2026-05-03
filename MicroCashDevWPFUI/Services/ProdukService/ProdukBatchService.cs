using MicroCashDevWPFUI.Data;
using MicroCashDevWPFUI.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroCashDevWPFUI.Services.ProdukService
{
	public class ProdukBatchService : IProdukBatchService
	{
		private readonly IAppDbContext _context;
		private const string VirtualPrefix = "VIR-BN-";

		public ProdukBatchService(IAppDbContext context)
		{
			_context = context;
		}

		// Ambil semua Batch (tanpa navigasi berat)
		public async Task<List<ProdukBatch>> GetAllAsync()
		{
			return await _context.ProdukBatchs
				.AsNoTracking()
				.ToListAsync();
		}

		// Ambil satu batch by Id
		public async Task<ProdukBatch?> GetByIdAsync(int id)
		{
			return await _context.ProdukBatchs
				.Include(pb => pb.ProdukSatuan)
				.ThenInclude(ps => ps.Produk)
				.FirstOrDefaultAsync(pb => pb.Id == id);
		}

		// Ambil semua batch berdasarkan produk satuan
		public async Task<List<ProdukBatch>> GetByProdukSatuanIdAsync(int produkSatuanId)
		{
			return await _context.ProdukBatchs
				.Where(pb => pb.ProdukSatuanId == produkSatuanId)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<List<ProdukBatch>> GetByDetailPembelianIdAsync(int detailPembelianId)
		{
			return await _context.ProdukBatchs
				.Where(pb => pb.DetailPembelianId == detailPembelianId)
				.AsNoTracking()
				.ToListAsync();
		}

		// Tambah batch
		public async Task<ProdukBatch> AddAsync(ProdukBatch batch)
		{
			if (string.IsNullOrWhiteSpace(batch.BatchNumber))
			{
				batch.BatchNumber = await GenerateVirtualBatchNumberAsync();
			}

			_context.ProdukBatchs.Add(batch);
			await _context.SaveChangesAsync();
			return batch;
		}

		// Tambah banyak batch sekaligus
		public async Task AddRangeAsync(IEnumerable<ProdukBatch> batchList)
		{
			foreach (var batch in batchList)
			{
				if (string.IsNullOrWhiteSpace(batch.BatchNumber))
				{
					batch.BatchNumber = await GenerateVirtualBatchNumberAsync();
				}
			}

			await _context.ProdukBatchs.AddRangeAsync(batchList);
			await _context.SaveChangesAsync();
		}

		// Update batch
		public async Task UpdateAsync(ProdukBatch batch)
		{
			_context.ProdukBatchs.Update(batch);
			await _context.SaveChangesAsync();
		}

		// Delete batch
		public async Task DeleteAsync(int id)
		{
			var entity = await _context.ProdukBatchs.FindAsync(id);
			if (entity != null)
			{
				_context.ProdukBatchs.Remove(entity);
				await _context.SaveChangesAsync();
			}
		}

		// Generate Virtual Batch Number
		public async Task<string> GenerateVirtualBatchNumberAsync()
		{
			var last = await _context.ProdukBatchs
				.Where(pb => pb.BatchNumber != null && pb.BatchNumber.StartsWith(VirtualPrefix))
				.OrderByDescending(pb => pb.Id)
				.Select(pb => pb.BatchNumber)
				.FirstOrDefaultAsync();

			int nextNum = 1;
			if (last != null && last.Length > VirtualPrefix.Length)
			{
				_ = int.TryParse(last.Substring(VirtualPrefix.Length), out nextNum);
				nextNum++;
			}

			return $"{VirtualPrefix}{nextNum:D4}";
		}
	}
}
