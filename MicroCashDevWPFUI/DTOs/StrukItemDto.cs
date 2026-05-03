using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.DTOs
{
	public class StrukItemDto
	{
		public string NamaBarang { get; set; } = string.Empty;
		public string Satuan { get; set; } = string.Empty;

		public int Jumlah { get; set; }
		public decimal Harga { get; set; }
		public decimal Subtotal { get; set; }
	}
	public class StrukDto
	{
		public string NamaToko { get; set; } = string.Empty;
		public string Alamat { get; set; } = string.Empty;
		public string? KataSambutan { get; set; }

		public string NomorNota { get; set; } = string.Empty;
		public DateTime Tanggal { get; set; }

		public decimal Total { get; set; }
		public decimal Dibayar { get; set; }
		public decimal Kembalian { get; set; }

		public List<StrukItemDto> Items { get; set; } = new();
	}
}
