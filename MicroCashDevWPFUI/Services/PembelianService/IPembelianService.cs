using MicroCashDevWPFUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Services.PembelianService
{
	public interface IPembelianService
	{
		Task<Pembelian> CreatePembelianAsync(Pembelian pembelian);
		Task<string> GenerateVirtualNomorFakturAsync();
		Task<Pembelian?> GetByIdAsync(int id);
		Task<List<Pembelian>> GetRiwayatAsync(DateTime? start = null, DateTime? end = null);
		Task UpdateAsync(Pembelian pembelian);
		Task<List<string>> GetNomorFakturBySupplierAsync(int supplierId);
		Task<Pembelian?> GetByNomorFakturAsync(string nomorFaktur);
	}
}
