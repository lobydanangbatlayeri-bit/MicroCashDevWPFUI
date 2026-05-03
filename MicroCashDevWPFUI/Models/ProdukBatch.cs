namespace MicroCashDevWPFUI.Models
{
	public class ProdukBatch
	{
		public int Id { get; set; }

		public int ProdukSatuanId { get; set; }
		public ProdukSatuan ProdukSatuan { get; set; } = null!;

		public int? DetailPembelianId { get; set; }
		public DetailPembelian? DetailPembelian { get; set; }

		public string? BatchNumber { get; set; }
		public DateTime TanggalMasuk { get; set; }
		public DateTime? TanggalKadarluasa { get; set; }

		public int Stok { get; set; }
	}

}
