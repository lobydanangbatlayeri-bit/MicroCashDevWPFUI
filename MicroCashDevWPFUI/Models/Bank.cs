using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Models
{
	public class Bank
	{
		public int Id { get; set; }
		public int SupplierId { get; set; }
		public Supplier Supplier { get; set; } = null!;

		public string NamaBank { get; set; } = "";
		public string NomorRekening { get; set; } = "";
	}

}
