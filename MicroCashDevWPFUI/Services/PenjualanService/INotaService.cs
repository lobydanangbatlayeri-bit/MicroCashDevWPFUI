using MicroCashDevWPFUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Services.PenjualanService
{
	public interface INotaService
	{
		Task<Nota> CreateAsync(Nota nota);
		Task<List<Nota>> GetByPembelianIdAsync(int pembelianId);
		Task UpdateAsync(Nota nota);
		Task DeleteAsync(int id);
		Task<bool> ExistsByPembelianIdAsync(int pembelianId);
		Task<Nota?> GetByIdAsync(int id);
		Task<List<Nota>> GetAllAsync(DateTime start, DateTime end);
	}
}
