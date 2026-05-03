namespace MicroCashDevWPFUI.Models
{
	public class Produk
	{
		public int Id { get; set; }
		public string NamaBarang { get; set; } = "";
		public bool IsDeleted { get; set; } = false;
		public DateTime? DeletedAt { get; set; }

		public ICollection<ProdukSatuan> ProdukSatuans { get; set; } = new List<ProdukSatuan>();
	}

}
