using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.CoreService;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public partial class DataUserViewModel : ObservableObject
	{
		private readonly IUserService _userService;
		private readonly IMenuService _menuService;
		private readonly IDialogService _dialogService;

		[ObservableProperty]
		private string? username;

		[ObservableProperty]
		private string? password;

		[ObservableProperty]
		private ObservableCollection<UserItem> itemsUser = new();

		public DataUserViewModel(
			IUserService userService,
			IMenuService menuService,
			IDialogService dialogService)
		{
			_userService = userService;
			_menuService = menuService;
			_dialogService = dialogService;

			LoadUsersCommand.Execute(null);
		}

		// ================= LOAD =================

		[RelayCommand]
		private async Task LoadUsers()
		{
			var users = await _userService.GetAllAsync();

			ItemsUser = new ObservableCollection<UserItem>(
				users.Select(u => new UserItem(_userService, u.Id)
				{
					Username = u.Username
				})
			);
		}

		// ================= CREATE =================

		[RelayCommand]
		private async Task Simpan()
		{
			if (string.IsNullOrWhiteSpace(Username) && string.IsNullOrWhiteSpace(Password))
			{
				await _dialogService.ShowMessage("Username dan Password wajib diisi.");
				return;
			}

			if (string.IsNullOrWhiteSpace(Username))
			{
				await _dialogService.ShowMessage("Username wajib diisi.");
				return;
			}

			if (string.IsNullOrWhiteSpace(Password))
			{
				await _dialogService.ShowMessage("Password wajib diisi.");
				return;
			}

			var existing = await _userService.GetByUsernameAsync(Username);
			if (existing != null)
			{
				await _dialogService.ShowMessage("Username sudah ada.");
				return;
			}

			var newUser = new User
			{
				Username = Username
			};

			newUser = await _userService.CreateAsync(newUser, Password);

			await AssignDefaultMenus(newUser.Id);

			ItemsUser.Add(new UserItem(_userService, newUser.Id)
			{
				Username = newUser.Username
			});

			Username = string.Empty;
			Password = string.Empty;
		}

		// ================= CANCEL =================
		[RelayCommand] 
		private async Task Cancel() 
		{ 
			Username = string.Empty;
			Password = string.Empty; 
		}

		// ================= DELETE =================

		[RelayCommand]
		private async Task DeleteUser(UserItem user)
		{
			if (user == null) return;

			try
			{
				var success = await _userService.DeleteAsync(user.Id);
				if (success)
				{
					ItemsUser.Remove(user);
				}
			}
			catch (InvalidOperationException ex)
			{
				await _dialogService.ShowMessage(ex.Message);
			}
		}

		// ================= DEFAULT MENU =================

		private async Task AssignDefaultMenus(int userId)
		{
			var allowedPageKeys = new[]
			{
				"DashboardPage",
				"TambahProdukPage",
				"ProdukPage",
				"RestokManualPage",
				"RestokNotaPage",
				"PenjualanPage",
				"TambahNotaPage",
				"NotaBelumBayarPage",
				"NotaSudahBayarPage"
			};

			var allMenus = await _menuService.GetAllMenusAsync();

			var menusToAssign = allMenus
				.Where(m => allowedPageKeys.Contains(m.PageKey))
				.ToList();

			var finalMenus = new List<Menu>();

			foreach (var menu in menusToAssign)
			{
				var current = menu;
				while (current != null)
				{
					if (!finalMenus.Contains(current))
						finalMenus.Add(current);

					current = current.Parent;
				}
			}

			foreach (var menu in finalMenus)
			{
				await _userService.AddUserMenuAsync(userId, menu.Id);
			}
		}
	}
}