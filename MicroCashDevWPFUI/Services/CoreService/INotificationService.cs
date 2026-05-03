using MicroCashDevWPFUI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Services.CoreService
{
	public interface INotificationService
	{
		Task<List<AppNotification>> GetNotificationsAsync();
	}
}
