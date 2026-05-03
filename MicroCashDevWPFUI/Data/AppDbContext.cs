using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.IO;

namespace MicroCashDevWPFUI.Data
{
	public class AppDbContext : DbContext, IAppDbContext
	{
		private readonly string _databasePath;

		public AppDbContext()
		{
			string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			string appFolder = Path.Combine(documentsPath, "MicroCash");

			if (!Directory.Exists(appFolder))
				Directory.CreateDirectory(appFolder);

			_databasePath = Path.Combine(appFolder, "MicroCash.db");
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite($"Data Source={_databasePath}");
		}

		// ======= DbSet ENTITIES =======
		public DbSet<User> Users => Set<User>();
		public DbSet<Menu> Menus => Set<Menu>();
		public DbSet<UserMenu> UserMenus => Set<UserMenu>();

		public DbSet<ProfileToko> ProfileTokos => Set<ProfileToko>();

		public DbSet<Produk> Produks => Set<Produk>();
		public DbSet<Satuan> Satuans => Set<Satuan>();
		public DbSet<ProdukSatuan> ProdukSatuans => Set<ProdukSatuan>();

		public DbSet<ProdukBatch> ProdukBatchs => Set<ProdukBatch>();

		public DbSet<Supplier> Suppliers => Set<Supplier>();
		public DbSet<Pembelian> Pembelians => Set<Pembelian>();
		public DbSet<DetailPembelian> DetailPembelians => Set<DetailPembelian>();

		public DbSet<Bank> Banks => Set<Bank>();
		public DbSet<Nota> Notas => Set<Nota>();

		public DbSet<Penjualan> Penjualans => Set<Penjualan>();
		public DbSet<DetailPenjualan> DetailPenjualans => Set<DetailPenjualan>();
		public DbSet<DetailPenjualanBatch> DetailPenjualanBatches => Set<DetailPenjualanBatch>();

		// Implementasi SaveChangesAsync untuk interface
		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			return await base.SaveChangesAsync(cancellationToken);
		}

		public Task<IDbContextTransaction> BeginTransactionAsync()
		{
			return Database.BeginTransactionAsync();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Produk>()
				.HasQueryFilter(p => !p.IsDeleted);

			base.OnModelCreating(modelBuilder);
		}

	}
}
