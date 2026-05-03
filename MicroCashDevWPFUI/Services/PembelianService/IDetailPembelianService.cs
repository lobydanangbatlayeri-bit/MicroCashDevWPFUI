using MicroCashDevWPFUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Services.PembelianService
{
	public interface IDetailPembelianService
	{
		Task<List<DetailPembelian>> GetAllAsync();
		Task<DetailPembelian?> GetByIdAsync(int id);
		Task<List<DetailPembelian>> GetByPembelianIdAsync(int pembelianId);

		Task<DetailPembelian> AddAsync(DetailPembelian detail);
		Task AddRangeAsync(IEnumerable<DetailPembelian> details);

		Task UpdateAsync(DetailPembelian detail);
		Task DeleteAsync(int id);
	}
}
