using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Models
{
	public class Satuan
	{
		public int Id { get; set; }
		public string NamaSatuan { get; set; } = "";

		public ICollection<ProdukSatuan> ProdukSatuans { get; set; } = new List<ProdukSatuan>();
	}


}
