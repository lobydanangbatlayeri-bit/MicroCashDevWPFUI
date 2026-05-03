using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.DTOs
{
	public enum NotificationType
	{
		Stock,
		Expired,
		DueInvoice,
		Info
	}

	public class AppNotification
	{
		public string Title { get; set; } = string.Empty;
		public string Message { get; set; } = string.Empty;
		public NotificationType Type { get; set; }
		public DateTime Date { get; set; }
	}
}
