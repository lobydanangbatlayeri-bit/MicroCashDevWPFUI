using MicroCashDevWPFUI.Data;
using MicroCashDevWPFUI.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Services.CoreService
{
	public class NotificationService : INotificationService
	{
		private readonly IAppDbContext _context;

		public NotificationService(IAppDbContext context)
		{
			_context = context;
		}

		public async Task<List<AppNotification>> GetNotificationsAsync()
		{
			var today = DateTime.Today;
			var result = new List<AppNotification>();

			// ==========================
			// 1️⃣ STOK ≤ 1
			// ==========================
			var lowStock = await _context.ProdukBatchs
				.Where(x =>
					x.Stok <= 1 &&
					!x.ProdukSatuan.Produk.IsDeleted)
				.Select(x => new
				{
					NamaBarang = x.ProdukSatuan.Produk.NamaBarang,
					Stok = x.Stok
				})
				.ToListAsync();

			foreach (var item in lowStock)
			{
				result.Add(new AppNotification
				{
					Title = "Stok Menipis",
					Message = $"{item.NamaBarang} sisa {item.Stok}",
					Type = NotificationType.Stock,
					Date = today
				});
			}

			// ==========================
			// 2️⃣ EXPIRED ≤ 7 hari ATAU sudah lewat
			// ==========================
			var expiredSoon = await _context.ProdukBatchs
				.Where(x =>
					x.Stok > 0 &&
					x.TanggalKadarluasa.HasValue &&
					x.TanggalKadarluasa.Value <= today.AddDays(7) &&
					!x.ProdukSatuan.Produk.IsDeleted)
				.Select(x => new
				{
					NamaBarang = x.ProdukSatuan.Produk.NamaBarang,
					Tanggal = x.TanggalKadarluasa!.Value,
					NomorFaktur = x.DetailPembelian!.Pembelian!.NomorFaktur
				})
				.ToListAsync();

			foreach (var item in expiredSoon)
			{
				var selisih = (item.Tanggal - today).Days;

				result.Add(new AppNotification
				{
					Title = "Kadaluarsa",
					Message = selisih >= 0
						? $"{item.NamaBarang} (Faktur {item.NomorFaktur}) expired dalam {selisih} hari"
						: $"{item.NamaBarang} (Faktur {item.NomorFaktur}) sudah expired {Math.Abs(selisih)} hari",
					Type = NotificationType.Expired,
					Date = item.Tanggal
				});
			}

			// ==========================
			// 3️⃣ NOTA JATUH TEMPO ≤ 2 hari ATAU sudah lewat
			// ==========================
			var batas = today.AddDays(2);

			var invoiceDue = await _context.Notas
				.Where(x =>
					x.TanggalDeadline <= batas &&
					!x.StatusPembayaran &&
					x.Pembelian != null)
				.Select(x => new
				{
					NomorFaktur = x.Pembelian.NomorFaktur,
					Deadline = x.TanggalDeadline
				})
				.ToListAsync();

			foreach (var nota in invoiceDue)
			{
				var selisih = (nota.Deadline - today).Days;

				result.Add(new AppNotification
				{
					Title = "Nota Jatuh Tempo",
					Message = selisih >= 0
						? $"Nota {nota.NomorFaktur} jatuh tempo {selisih} hari lagi"
						: $"Nota {nota.NomorFaktur} terlambat {Math.Abs(selisih)} hari",
					Type = NotificationType.DueInvoice,
					Date = nota.Deadline
				});
			}

			if (result.Count == 0)
			{
				result.Add(new AppNotification
				{
					Title = "Notifikasi",
					Message = "Tidak ada Notifikasi",
					Type = NotificationType.Info,
					Date = today
				});
			}

			return result
				.OrderBy(x => x.Date)
				.ToList();
		}
	}
}
