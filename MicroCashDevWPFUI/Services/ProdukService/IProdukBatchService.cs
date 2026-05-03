using MicroCashDevWPFUI.Models;

namespace MicroCashDevWPFUI.Services.ProdukService
{
	public interface IProdukBatchService
	{
		Task<List<ProdukBatch>> GetAllAsync();
		Task<ProdukBatch?> GetByIdAsync(int id);
		Task<List<ProdukBatch>> GetByProdukSatuanIdAsync(int produkSatuanId);
		Task<List<ProdukBatch>> GetByDetailPembelianIdAsync(int detailPembelianId);

		Task<ProdukBatch> AddAsync(ProdukBatch batch);
		Task AddRangeAsync(IEnumerable<ProdukBatch> batchList);

		Task UpdateAsync(ProdukBatch batch);
		Task DeleteAsync(int id);

		Task<string> GenerateVirtualBatchNumberAsync();
	}
}
