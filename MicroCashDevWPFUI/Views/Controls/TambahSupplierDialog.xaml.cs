using MicroCashDevWPFUI.DTOs;
using System.Windows.Controls;

namespace MicroCashDevWPFUI.Views.Controls
{
    public partial class TambahSupplierDialog : UserControl
    {
        public TambahSupplierDialog()
        {
            InitializeComponent();
        }

        private void dgSupplier_CurrentCellChanged(object sender, EventArgs e)
        {
            var grid = (DataGrid)sender;
            grid.CommitEdit(DataGridEditingUnit.Row, true);
		}

        private void dgSupplier_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Row.Item is SupplierItem item)
            {
                item.IsDirty = true;
            }
		}
	}
}
