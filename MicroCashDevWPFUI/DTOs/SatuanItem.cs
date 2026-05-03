using CommunityToolkit.Mvvm.ComponentModel;

namespace MicroCashDevWPFUI.DTOs
{
	public partial class SatuanItem : ObservableObject
	{
		[ObservableProperty]
		private int nomor;

		[ObservableProperty]
		private int id;

		[ObservableProperty]
		private string namaSatuan = string.Empty;

		public bool IsDirty { get; set; }

		partial void OnNamaSatuanChanged(string value)
		{
			IsDirty = true;
		}
	}
}
