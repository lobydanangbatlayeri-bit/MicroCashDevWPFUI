using MicroCashDevWPFUI.Data;
using MicroCashDevWPFUI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Services.CoreService
{
	public class MenuService : IMenuService
	{
		private readonly IAppDbContext _dbContext;

		public MenuService(IAppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<Menu>> GetMenusForUserAsync(int userId)
		{
			// Ambil menu root untuk user tertentu
			var menus = await _dbContext.Menus
				.Where(m => m.UserMenus.Any(um => um.UserId == userId) && m.ParentId == null)
				.OrderBy(m => m.Urutan)
				.ToListAsync();

			// Optional: load children secara rekursif jika perlu
			foreach (var menu in menus)
			{
				await LoadChildrenAsync(menu, userId);
			}

			return menus;
		}

		private async Task LoadChildrenAsync(Menu menu, int userId)
		{
			menu.Children = await _dbContext.Menus
				.Where(m => m.ParentId == menu.Id &&
							m.UserMenus.Any(um => um.UserId == userId))
				.OrderBy(m => m.Urutan)
				.ToListAsync();

			foreach (var child in menu.Children)
			{
				await LoadChildrenAsync(child, userId);
			}
		}

		public async Task<List<Menu>> GetAllMenusAsync()
		{
			return await _dbContext.Menus
				.Include(m => m.Parent)
				.Include(m => m.Children)
				.ToListAsync();
		}

	}
}
