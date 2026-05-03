using MicroCashDevWPFUI.Models;

namespace MicroCashDevWPFUI.Services.PenjualanService
{
	public interface IDetailPenjualanService
	{
		Task<List<DetailPenjualan>> GetAllAsync();
		Task<DetailPenjualan?> GetByIdAsync(int id);
		Task<List<DetailPenjualan>> GetByPenjualanIdAsync(int penjualanId);

		Task<DetailPenjualan> AddAsync(DetailPenjualan detail);
		Task AddRangeAsync(IEnumerable<DetailPenjualan> details);
		Task UpdateAsync(DetailPenjualan detail);
		Task DeleteAsync(int id);
	}
}
