using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Models
{
	public class DetailPenjualan
	{
		public int Id { get; set; }

		public int PenjualanId { get; set; }
		public Penjualan Penjualan { get; set; } = null!;

		public int ProdukSatuanId { get; set; }
		public ProdukSatuan ProdukSatuan { get; set; } = null!;

		public int Jumlah { get; set; }
		public decimal Harga { get; set; }
		public decimal Subtotal { get; set; }

		public ICollection<DetailPenjualanBatch> DetailPenjualanBatches { get; set; } = new List<DetailPenjualanBatch>();
	}

}
