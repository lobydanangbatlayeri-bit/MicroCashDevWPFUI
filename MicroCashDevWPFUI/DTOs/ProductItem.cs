using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.DTOs
{
	public class ProductItem
	{
		public int Id { get; set; }
		public string NamaBarang { get; set; } = "";

		// Optional (kalau mau bantu matching dosis)
		public string? KeywordNormalized { get; set; }
	}
}
