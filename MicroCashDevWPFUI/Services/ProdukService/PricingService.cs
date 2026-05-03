using MicroCashDevWPFUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Services.ProdukService
{
	public class PricingService : IPricingService
	{
		public decimal GetEstimasiHargaBeli(ProdukSatuan satuan)
		{
			if (satuan?.Produk?.ProdukSatuans == null || !satuan.Produk.ProdukSatuans.Any())
				return 0;

			var satuanValid = satuan.Produk.ProdukSatuans
				.Where(s => s.HargaJual > 0)
				.OrderBy(s => Math.Abs(s.JumlahPerSatuan - satuan.JumlahPerSatuan))
				.FirstOrDefault();

			if (satuanValid == null)
				return 0;

			decimal hargaPerUnit = satuanValid.HargaJual / satuanValid.JumlahPerSatuan;

			return hargaPerUnit * satuan.JumlahPerSatuan;
		}
	}
}
