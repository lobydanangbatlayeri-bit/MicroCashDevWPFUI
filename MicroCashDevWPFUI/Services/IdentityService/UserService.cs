using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Data;
using MicroCashDevWPFUI.Helpers;
using Microsoft.EntityFrameworkCore;
using MicroCashDevWPFUI.Services.CoreService;

namespace MicroCashDevWPFUI.Services.IdentityService
{
	public class UserService : IUserService
	{
		private readonly IAppDbContext _context;

		public UserService(IAppDbContext context)
		{
			_context = context;
		}

		public async Task<User?> LoginAsync(string username, string password)
		{
			var user = await _context.Users
				.FirstOrDefaultAsync(x => x.Username == username);

			if (user == null)
				return null;

			if (!PasswordHelper.VerifyPassword(user.Password, password))
				return null;

			return user;
		}

		public async Task<User?> GetByIdAsync(int id)
		{
			return await _context.Users
				.Include(x => x.UserMenus)
				.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<List<User>> GetAllAsync()
		{
			return await _context.Users
				.Include(x => x.UserMenus)
				.ToListAsync();
		}

		public async Task<User?> GetByUsernameAsync(string username)
		{
			return await _context.Users
				.FirstOrDefaultAsync(x => x.Username == username);
		}

		public async Task<User> CreateAsync(User user, string password)
		{
			user.Password = PasswordHelper.HashPassword(password);

			await _context.Users.AddAsync(user);
			await _context.SaveChangesAsync();

			return user;
		}

		public async Task<bool> UpdateAsync(User user)
		{
			var existing = await _context.Users.FindAsync(user.Id);
			if (existing == null) return false;

			// Update TANPA menyentuh password
			existing.Username = user.Username;
			existing.UserMenus = user.UserMenus;

			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> ChangePasswordAsync(int userId, string newPassword)
		{
			var user = await _context.Users.FindAsync(userId);
			if (user == null) return false;

			user.Password = PasswordHelper.HashPassword(newPassword);

			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var user = await _context.Users
				.Include(u => u.UserMenus) // pastikan relasi UserMenus ikut dihapus
				.FirstOrDefaultAsync(u => u.Id == id);

			if (user == null) return false;

			// Cek transaksi penjualan
			bool canDelete = await CanDeleteUserAsync(user.Id);
			if (!canDelete)
				throw new InvalidOperationException(
					"User tidak dapat dihapus karena memiliki riwayat transaksi.");

			_context.UserMenus.RemoveRange(user.UserMenus);
			_context.Users.Remove(user);

			return await _context.SaveChangesAsync() > 0;
		}

		public async Task AddUserMenuAsync(int userId, int menuId)
		{
			var userMenu = new UserMenu
			{
				UserId = userId,
				MenuId = menuId
			};
			_context.UserMenus.Add(userMenu);
			await _context.SaveChangesAsync();
		}

		public async Task<bool> CanDeleteUserAsync(int userId)
		{
			// Cek apakah user pernah melakukan penjualan
			bool hasPenjualan = await _context.Penjualans
				.AnyAsync(p => p.UserId == userId);

			return !hasPenjualan;
		}

		public async Task<List<Menu>> GetMenusForUserAsync(int userId)
		{
			return await _context.UserMenus
				.Where(um => um.UserId == userId)
				.Include(um => um.Menu)
				.Select(um => um.Menu!)
				.ToListAsync();
		}

		public async Task SetUserMenusAsync(int userId, List<int> menuIds)
		{
			// Hapus menu lama
			var existing = _context.UserMenus.Where(um => um.UserId == userId);
			_context.UserMenus.RemoveRange(existing);

			// Tambahkan menu baru
			var newUserMenus = menuIds.Select(id => new UserMenu
			{
				UserId = userId,
				MenuId = id
			});

			await _context.UserMenus.AddRangeAsync(newUserMenus);
			await _context.SaveChangesAsync();
		}

	}
}
