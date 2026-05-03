using MicroCashDevWPFUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Services.ProdukService
{
	public interface IProdukService
	{
		Task<List<Produk>> GetAllAsync();
		Task<Produk?> GetByIdAsync(int id);
		Task<Produk?> GetByNamaAsync(string namaBarang);
		Task<Produk> AddAsync(Produk produk);
		Task UpdateAsync(Produk produk);
		Task DeleteAsync(int id);
	}
}
