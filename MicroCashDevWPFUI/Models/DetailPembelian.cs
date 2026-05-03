namespace MicroCashDevWPFUI.Models
{
	public class DetailPembelian
	{
		public int Id { get; set; }

		public int PembelianId { get; set; }
		public Pembelian Pembelian { get; set; } = null!;

		public int ProdukSatuanId { get; set; }
		public ProdukSatuan ProdukSatuan { get; set; } = null!;

		public int Jumlah { get; set; }
		public decimal HargaBeli { get; set; }
		public decimal Subtotal { get; set; }

		public ICollection<ProdukBatch> ProdukBatchs { get; set; } = new List<ProdukBatch>();
	}

}
