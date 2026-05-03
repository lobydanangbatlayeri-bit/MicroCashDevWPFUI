using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.DTOs
{
    public partial class SupplierItem : ObservableObject
	{
		[ObservableProperty]
		private int nomor;

		[ObservableProperty]
		private int id;

		[ObservableProperty]
		private string namaSupplier = string.Empty;

		[ObservableProperty]
		private string alamat = string.Empty;

		public bool IsDirty { get; set; }

		partial void OnNamaSupplierChanged(string value)
		{
			IsDirty = true;
		}

	}
}
