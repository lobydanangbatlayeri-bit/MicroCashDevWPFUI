using MicroCashDevWPFUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Services.CoreService
{
	public interface IMenuService
	{
		Task<List<Menu>> GetMenusForUserAsync(int userId);
		Task<List<Menu>> GetAllMenusAsync();
	}
}
