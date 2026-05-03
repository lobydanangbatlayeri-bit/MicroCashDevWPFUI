using MicroCashDevWPFUI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Services.PembelianService
{
	public interface IRestokService
	{
		Task RestokProdukAsync(int produkSatuanId, int jumlah, DateTime tanggalKadaluarsa, decimal hargaBeli);
		Task RestokNotaAsync(int supplierId,
					 List<RestokNotaItemDto> items,
					 decimal totalNota,
					 string? nomorFaktur,
					 DateTime tanggalRestok);
	}
}
