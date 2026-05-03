using MicroCashDevWPFUI.DTOs;
using System.Windows.Controls;

namespace MicroCashDevWPFUI.Views.Controls
{
	public partial class TambahSatuanDialog : UserControl
	{
		public TambahSatuanDialog()
		{
			InitializeComponent();
		}

		private void dgSatuan_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			if (e.Row.Item is SatuanItem item)
			{
				item.IsDirty = true;
			}
		}

		private void dgSatuan_CurrentCellChanged(object sender, EventArgs e)
		{
			var grid = (DataGrid)sender;
			grid.CommitEdit(DataGridEditingUnit.Row, true);
		}
	}
}
