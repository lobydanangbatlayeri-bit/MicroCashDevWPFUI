using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.DTOs
{
	public class RestokNotaScanItemDto
	{
		public string NamaBarang { get; set; } = string.Empty;
		public int Jumlah { get; set; }
		public decimal HargaBeli { get; set; }
		public decimal SubTotal { get; set; }
		public DateTime? TanggalKadarluasa { get; set; }
		public string? NomorBatch { get; set; }
	}
}
