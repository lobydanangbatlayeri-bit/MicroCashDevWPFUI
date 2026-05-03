using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Models
{
	public class Nota
	{
		public int Id { get; set; }

		public int PembelianId { get; set; }
		public Pembelian Pembelian { get; set; } = null!;

		public bool StatusPembayaran { get; set; } = false;
		public DateTime TanggalDeadline { get; set; }
		public decimal Total { get; set; }
	}

}
