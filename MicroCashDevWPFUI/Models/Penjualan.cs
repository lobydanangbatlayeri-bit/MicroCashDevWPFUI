using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Models
{
	public class Penjualan
	{
		public int Id { get; set; }

		public int UserId { get; set; }
		public User User { get; set; } = null!;

		public string NomorNota { get; set; } = "";
		public DateTime Tanggal { get; set; }

		public decimal Total { get; set; }
		public decimal Dibayar { get; set; }
		public decimal Kembalian { get; set; }

		public ICollection<DetailPenjualan> DetailPenjualans { get; set; } = new List<DetailPenjualan>();
	}

}
