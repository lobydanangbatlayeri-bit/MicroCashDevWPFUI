using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.DTOs
{
	public class RecentSaleItem
	{
		public string NomorNota { get; set; } = string.Empty;
		public DateTime Tanggal { get; set; }
		public decimal Total { get; set; }
	}

	public class LowStockItem
	{
		public string NamaProduk { get; set; } = string.Empty;
		public int Stok { get; set; }
	}
}
