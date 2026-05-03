using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.DTOs
{
	public partial class BankItem : ObservableObject
	{
		[ObservableProperty]
		private int nomor;

		[ObservableProperty]
		private int id;

		[ObservableProperty]
		private int supplierId;

		[ObservableProperty]
		private string namaBank = string.Empty;

		[ObservableProperty]
		private string nomorRekening = string.Empty;
	}
}
