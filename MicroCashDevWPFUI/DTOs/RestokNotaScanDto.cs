using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.DTOs
{
	public class RestokNotaScanDto
	{
		public string SupplierNama { get; set; } = string.Empty;
		public string NomorNota { get; set; } = string.Empty;
		public DateTime TanggalRestok { get; set; }
		public decimal Total { get; set; }

		public List<RestokNotaScanItemDto> Items { get; set; } = new();
	}
}
