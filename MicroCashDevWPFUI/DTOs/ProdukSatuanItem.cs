using CommunityToolkit.Mvvm.ComponentModel;

namespace MicroCashDevWPFUI.DTOs
{
	public partial class ProdukSatuanItem : ObservableObject
	{
		[ObservableProperty]
		private int produkId;

		[ObservableProperty]
		private int produkSatuanId;

		[ObservableProperty]
		private SatuanItem? satuan;

		[ObservableProperty]
		public int isiPerBox = 1;

		[ObservableProperty]
		public decimal hargaJual;

		[ObservableProperty]
		private bool isSelected;
	}

}
