using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Models
{
	public class DetailPenjualanBatch
	{
		public int Id { get; set; }

		public int DetailPenjualanId { get; set; }
		public DetailPenjualan? DetailPenjualan { get; set; }

		public int ProdukBatchId { get; set; }
		public ProdukBatch? ProdukBatch { get; set; }

		public int Jumlah { get; set; }
		public decimal HargaBeli { get; set; }
	}

}
