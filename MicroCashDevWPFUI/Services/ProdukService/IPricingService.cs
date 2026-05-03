using MicroCashDevWPFUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Services.ProdukService
{
	public interface IPricingService
	{
		decimal GetEstimasiHargaBeli(ProdukSatuan satuan);
	}
}
