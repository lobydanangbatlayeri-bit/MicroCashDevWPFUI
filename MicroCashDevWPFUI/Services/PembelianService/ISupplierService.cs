using MicroCashDevWPFUI.Models;

namespace MicroCashDevWPFUI.Services.PembelianService
{
	public interface ISupplierService
	{
		Task<List<Supplier>> GetAllAsync();
		Task<Supplier?> GetByIdAsync(int id);
		Task<Supplier> AddAsync(Supplier supplier);
		Task<Supplier> UpdateAsync(Supplier supplier);
		Task<bool> DeleteAsync(int id);

		Task<List<Supplier>> SearchAsync(string keyword);
	}
}
