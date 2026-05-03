using MicroCashDevWPFUI.Data;
using MicroCashDevWPFUI.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroCashDevWPFUI.Services.PenjualanService
{
	public class NotaService : INotaService
	{
		private readonly IAppDbContext _db;

		public NotaService(IAppDbContext db)
		{
			_db = db;
		}

		public async Task<Nota> CreateAsync(Nota nota)
		{
			_db.Notas.Add(nota);
			await _db.SaveChangesAsync();
			return nota;
		}

		public async Task<List<Nota>> GetByPembelianIdAsync(int pembelianId)
		{
			return await _db.Notas
				.Where(n => n.PembelianId == pembelianId)
				.Include(n => n.Pembelian)
					.ThenInclude(p => p.Supplier)
						.ThenInclude(s => s.Banks)
				.ToListAsync();
		}

		public async Task UpdateAsync(Nota nota)
		{
			var existing = await _db.Notas.FindAsync(nota.Id);
			if (existing == null)
				throw new Exception("Nota tidak ditemukan");

			_db.Entry(existing).CurrentValues.SetValues(nota);
			await _db.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var nota = await _db.Notas.FindAsync(id);
			if (nota == null) return;

			_db.Notas.Remove(nota);
			await _db.SaveChangesAsync();
		}

		public async Task<bool> ExistsByPembelianIdAsync(int pembelianId)
		{
			return await _db.Notas.AnyAsync(n => n.PembelianId == pembelianId);
		}

		public async Task<Nota?> GetByIdAsync(int id)
		{
			return await _db.Notas
				.Include(n => n.Pembelian)
					.ThenInclude(p => p.Supplier)
						.ThenInclude(s => s.Banks)
				.FirstOrDefaultAsync(n => n.Id == id);
		}

		public async Task<List<Nota>> GetAllAsync(DateTime start, DateTime end)
		{
			return await _db.Notas
				.Include(n => n.Pembelian)
					.ThenInclude(p => p.Supplier)
						.ThenInclude(s => s.Banks)
				.Where(n => n.TanggalDeadline >= start && n.TanggalDeadline <= end)
				.OrderBy(n => n.TanggalDeadline)
				.ToListAsync();
		}

	}
}
