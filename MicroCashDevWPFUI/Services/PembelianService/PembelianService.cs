using MicroCashDevWPFUI.Data;
using MicroCashDevWPFUI.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroCashDevWPFUI.Services.PembelianService
{
	public class PembelianService : IPembelianService
	{
		private readonly IAppDbContext _db;
		private const string VirtualPrefix = "VIR-FK-";

		public PembelianService(IAppDbContext db)
		{
			_db = db;
		}

		public async Task<Pembelian> CreatePembelianAsync(Pembelian pembelian)
		{
			if (string.IsNullOrWhiteSpace(pembelian.NomorFaktur))
				pembelian.NomorFaktur = await GenerateVirtualNomorFakturAsync();

			_db.Pembelians.Add(pembelian);
			await _db.SaveChangesAsync();
			return pembelian;
		}

		public async Task<string> GenerateVirtualNomorFakturAsync()
		{
			var lastNomor = await _db.Pembelians
				.Where(p => p.NomorFaktur.StartsWith(VirtualPrefix))
				.OrderByDescending(p => p.Id)
				.Select(p => p.NomorFaktur)
				.FirstOrDefaultAsync();

			int nextNum = 1;
			if (!string.IsNullOrWhiteSpace(lastNomor) && lastNomor.Length > VirtualPrefix.Length)
			{
				if (int.TryParse(lastNomor.Substring(VirtualPrefix.Length), out int lastNumber))
				{
					nextNum = lastNumber + 1;
				}
			}

			return $"{VirtualPrefix}{nextNum:D4}";
		}

		public async Task<Pembelian?> GetByIdAsync(int id)
		{
			return await _db.Pembelians
				.Include(p => p.DetailPembelians)
				.Include(p => p.Supplier)
				.FirstOrDefaultAsync(p => p.Id == id);
		}

		public async Task<List<Pembelian>> GetRiwayatAsync(DateTime? start = null, DateTime? end = null)
		{
			var query = _db.Pembelians.AsQueryable();

			if (start != null)
				query = query.Where(x => x.Tanggal >= start);

			if (end != null)
				query = query.Where(x => x.Tanggal <= end);

			return await query
				.Include(p => p.Supplier)
				.OrderByDescending(x => x.Tanggal)
				.ToListAsync();
		}

		public async Task UpdateAsync(Pembelian pembelian)
		{
			// Pastikan entity sudah di-track
			var tracked = await _db.Pembelians
				.Include(p => p.Supplier)
				.Include(p => p.DetailPembelians)
				.FirstOrDefaultAsync(p => p.Id == pembelian.Id);

			if (tracked != null)
			{
				// Update properti
				tracked.NomorFaktur = pembelian.NomorFaktur;
				tracked.Tanggal = pembelian.Tanggal;
				tracked.Supplier = pembelian.Supplier;
				tracked.Total = pembelian.Total;

				// Detail pembelian tetap, kita update lewat DetailPembelianService
				// Jika nanti ingin update dari sini juga, bisa loop tracked.DetailPembelians

				await _db.SaveChangesAsync();
			}
			else
			{
				// Kalau tidak ada, bisa log atau throw error
				throw new InvalidOperationException($"Pembelian dengan Id={pembelian.Id} tidak ditemukan.");
			}
		}

		public async Task<List<string>> GetNomorFakturBySupplierAsync(int supplierId)
		{
			return await _db.Pembelians
				.Where(p => p.SupplierId == supplierId)
				.OrderByDescending(p => p.Tanggal)
				.Select(p => p.NomorFaktur)
				.ToListAsync();
		}

		public async Task<Pembelian?> GetByNomorFakturAsync(string nomorFaktur)
		{
			return await _db.Pembelians
				.Include(p => p.Supplier)
				.FirstOrDefaultAsync(p => p.NomorFaktur == nomorFaktur);
		}

	}
}
