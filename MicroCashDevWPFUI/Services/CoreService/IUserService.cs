using MicroCashDevWPFUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Services.CoreService
{
	public interface IUserService
	{
		Task<User?> LoginAsync(string username, string password);

		Task<User?> GetByIdAsync(int id);

		Task<bool> ChangePasswordAsync(int userId, string newPassword);

		Task<User?> GetByUsernameAsync(string username);

		Task<List<User>> GetAllAsync();

		Task<User> CreateAsync(User user, string password);

		Task<bool> UpdateAsync(User user);

		Task<bool> DeleteAsync(int id);

		Task AddUserMenuAsync(int userId, int menuId);

		Task<bool> CanDeleteUserAsync(int userId);

		Task<List<Menu>> GetMenusForUserAsync(int userId);

		Task SetUserMenusAsync(int userId, List<int> menuIds);

	}
}
