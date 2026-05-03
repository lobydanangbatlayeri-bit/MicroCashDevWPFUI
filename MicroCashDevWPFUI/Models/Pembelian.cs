using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Models
{
	public class Pembelian
	{
		public int Id { get; set; }

		public int SupplierId { get; set; }
		public Supplier Supplier { get; set; } = null!;

		public string NomorFaktur { get; set; } = "";
		public DateTime Tanggal { get; set; }
		public decimal Total { get; set; }

		public ICollection<DetailPembelian> DetailPembelians { get; set; } = new List<DetailPembelian>();
		public ICollection<Nota> Notas { get; set; } = new List<Nota>();
	}

}
