using MicroCashDevWPFUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Services.ProdukService
{
	public interface IProdukSatuanService
	{
		Task<List<ProdukSatuan>> GetAllAsync();
		Task<ProdukSatuan?> GetByIdAsync(int id);
		Task<List<ProdukSatuan>> GetByProdukIdAsync(int produkId);
		Task AddAsync(ProdukSatuan produkSatuan);
		Task UpdateAsync(ProdukSatuan produkSatuan);
		Task DeleteAsync(int id);
		Task AddRangeAsync(IEnumerable<ProdukSatuan> produkSatuanList);
	}
}
