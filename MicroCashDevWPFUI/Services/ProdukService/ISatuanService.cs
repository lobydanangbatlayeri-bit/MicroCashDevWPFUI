using MicroCashDevWPFUI.Models;

namespace MicroCashDevWPFUI.Services.ProdukService
{
	public interface ISatuanService
	{
		Task<IEnumerable<Satuan>> GetAllAsync();
		Task<Satuan?> GetByIdAsync(int id);
		Task<Satuan> AddAsync(Satuan satuan);
		Task<bool> UpdateAsync(Satuan satuan);
		Task<bool> DeleteAsync(int id);
	}
}
