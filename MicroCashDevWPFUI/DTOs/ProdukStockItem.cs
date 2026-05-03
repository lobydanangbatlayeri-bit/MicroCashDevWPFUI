using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.DTOs
{
	public partial class ProdukStockItem : ObservableObject
	{
		[ObservableProperty]
		private int produkSatuanId;

		[ObservableProperty]
		private string? namaBarang;

		[ObservableProperty]
		private int jumlah;

		[ObservableProperty]
		private decimal hargaBeli;

		[ObservableProperty]
		private decimal subTotal;

		[ObservableProperty]
		private string? nomorBatch;

		[ObservableProperty]
		private DateTime? tanggalMasuk;

		[ObservableProperty]
		private DateTime? tanggalKadarluasa;
	}

}
