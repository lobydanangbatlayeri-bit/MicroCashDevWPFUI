using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.DTOs
{
	public partial class KeranjangItems : ObservableObject
	{
		[ObservableProperty]
		private int produkId;
		[ObservableProperty]
		private int produkSatuanId;
		[ObservableProperty]
		private string namaBarang = string.Empty;
		[ObservableProperty]
		private SatuanItem? satuan;
		[ObservableProperty]
		private decimal hargaJual;
		[ObservableProperty]
		private int jumlah;
		[ObservableProperty]
		private decimal subTotal;

		partial void OnJumlahChanged(int value)
		{
			SubTotal = value * HargaJual;
		}

		partial void OnHargaJualChanged(decimal value)
		{
			SubTotal = value * Jumlah;
		}
	}
}
