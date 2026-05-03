using MicroCashDevWPFUI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace MicroCashDevWPFUI.Data
{
	public interface IAppDbContext
    {
		DbSet<User> Users { get; }
		DbSet<Menu> Menus { get; }
		DbSet<UserMenu> UserMenus { get; }

		DbSet<ProfileToko> ProfileTokos { get; }

		DbSet<Produk> Produks { get; }
		DbSet<Satuan> Satuans { get; }
		DbSet<ProdukSatuan> ProdukSatuans { get; }

		DbSet<ProdukBatch> ProdukBatchs { get; }

		DbSet<Supplier> Suppliers { get; }
		DbSet<Pembelian> Pembelians { get; }
		DbSet<DetailPembelian> DetailPembelians { get; }

		DbSet<Bank> Banks { get; }
		DbSet<Nota> Notas { get; }

		DbSet<Penjualan> Penjualans { get; }
		DbSet<DetailPenjualan> DetailPenjualans { get; }
		DbSet<DetailPenjualanBatch> DetailPenjualanBatches { get; }

		// Method untuk menyimpan perubahan
		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
		DbSet<TEntity> Set<TEntity>() where TEntity : class;
		EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

		Task<IDbContextTransaction> BeginTransactionAsync();
	}
}
