namespace MicroCashDevWPFUI.Models
{
	public class ProdukSatuan
	{
		public int Id { get; set; }

		public int ProdukId { get; set; }
		public Produk Produk { get; set; } = null!;

		public int SatuanId { get; set; }
		public Satuan Satuan { get; set; } = null!;

		public int JumlahPerSatuan { get; set; }
		public decimal HargaJual { get; set; }

		public ICollection<ProdukBatch> ProdukBatchs { get; set; } = new List<ProdukBatch>();
		public ICollection<DetailPembelian> DetailPembelians { get; set; } = new List<DetailPembelian>();
		public ICollection<DetailPenjualan> DetailPenjualans { get; set; } = new List<DetailPenjualan>();
	}

}
