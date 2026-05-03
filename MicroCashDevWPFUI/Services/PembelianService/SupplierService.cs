using MicroCashDevWPFUI.Data;
using MicroCashDevWPFUI.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroCashDevWPFUI.Services.PembelianService
{
	public class SupplierService : ISupplierService
	{
		private readonly IAppDbContext _db;

		public SupplierService(IAppDbContext db)
		{
			_db = db;
		}

		public async Task<List<Supplier>> GetAllAsync()
			=> await _db.Suppliers
						.Where(x => x.Id > 1)
						.ToListAsync();

		public async Task<Supplier?> GetByIdAsync(int id)
			=> await _db.Suppliers.FindAsync(id);

		public async Task<Supplier> AddAsync(Supplier supplier)
		{
			_db.Suppliers.Add(supplier);
			await _db.SaveChangesAsync();
			return supplier;
		}

		public async Task<Supplier> UpdateAsync(Supplier supplier)
		{
			var existing = await _db.Suppliers
									.FirstAsync(x => x.Id == supplier.Id);

			// copy nilai
			_db.Entry(existing).CurrentValues.SetValues(supplier);

			await _db.SaveChangesAsync();
			return existing;
		}


		public async Task<bool> DeleteAsync(int id)
		{
			var supplier = await GetByIdAsync(id);
			if (supplier == null)
				return false;

			// ✅ CEK apakah sudah ada transaksi
			bool sudahDipakai = await _db.Pembelians
										 .AnyAsync(x => x.SupplierId == id);

			if (sudahDipakai)
				return false; // tidak boleh dihapus

			_db.Suppliers.Remove(supplier);
			await _db.SaveChangesAsync();
			return true;
		}

		public async Task<List<Supplier>> SearchAsync(string keyword)
		{
			return await _db.Suppliers
				.Where(s => s.NamaSupplier.Contains(keyword))
				.ToListAsync();
		}
	}
}
