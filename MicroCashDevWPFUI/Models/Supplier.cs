using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Models
{
	public class Supplier
	{
		public int Id { get; set; }
		public string NamaSupplier { get; set; } = "";
		public string Alamat { get; set; } = "";

		public ICollection<Pembelian> Pembelians { get; set; } = new List<Pembelian>();
		public ICollection<Bank> Banks { get; set; } = new List<Bank>();
	}

}
