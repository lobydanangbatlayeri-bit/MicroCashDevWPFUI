using MicroCashDevWPFUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Services.KeuanganService
{
	public interface IBankService
	{
		Task<Bank> AddAsync(Bank bank);
		Task<List<Bank>> GetBySupplierIdAsync(int supplierId);
		Task DeleteAsync(int bankId);
	}
}
