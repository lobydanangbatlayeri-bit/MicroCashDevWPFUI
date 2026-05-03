using MicroCashDevWPFUI.Data;
using MicroCashDevWPFUI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Services.KeuanganService
{
	public class BankService : IBankService
	{
		private readonly IAppDbContext _db;

		public BankService(IAppDbContext db)
		{
			_db = db;
		}

		public async Task<Bank> AddAsync(Bank bank)
		{
			_db.Banks.Add(bank);
			await _db.SaveChangesAsync();
			return bank;
		}

		public async Task<List<Bank>> GetBySupplierIdAsync(int supplierId)
		{
			return await _db.Banks
				.Where(b => b.SupplierId == supplierId)
				.ToListAsync();
		}

		public async Task DeleteAsync(int bankId)
		{
			var bank = await _db.Banks.FindAsync(bankId);
			if (bank == null) return;

			_db.Banks.Remove(bank);
			await _db.SaveChangesAsync();
		}

	}

}
