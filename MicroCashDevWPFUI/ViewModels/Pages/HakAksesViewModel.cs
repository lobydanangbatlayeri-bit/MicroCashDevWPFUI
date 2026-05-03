using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using MicroCashDevWPFUI.Services.CoreService;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public partial class HakAksesViewModel : ObservableObject
	{
		private readonly IUserService _userService;
		private readonly IMenuService _menuService;

		// ===== Bindings =====
		[ObservableProperty] private ObservableCollection<User> itemsUser = new();
		[ObservableProperty] private User? selectedUser;
		[ObservableProperty] private ObservableCollection<MenuCheckItem> menus = new();

		public HakAksesViewModel(IUserService userService, IMenuService menuService)
		{
			_userService = userService;
			_menuService = menuService;

			LoadUsersCommand.Execute(null);
		}

		// ===== Load all users =====
		[RelayCommand]
		private async Task LoadUsers()
		{
			var users = await _userService.GetAllAsync();
			ItemsUser = new ObservableCollection<User>(users);
		}

		// ===== User selected from ComboBox =====
		[RelayCommand]
		private async Task UserSelected()
		{
			if (SelectedUser == null) return;

			// Load all menus
			var allMenus = await _menuService.GetAllMenusAsync();

			// SubscribeAutoSave tetap seperti sebelumnya, tinggal pakai FromMenuModel baru
			var rootMenus = allMenus
				.Where(m => m.ParentId == null)
				.OrderBy(m => m.Urutan)
				.Select(m => MenuCheckItem.FromMenuModel(m)); // parent-child sudah diset
			Menus = new ObservableCollection<MenuCheckItem>(rootMenus);
			SubscribeAutoSave(Menus); // auto-save tetap aktif

			Menus = new ObservableCollection<MenuCheckItem>(rootMenus);

			// Tandai menu yang sudah dipilih user
			var userMenuIds = (await _userService.GetMenusForUserAsync(SelectedUser.Id))
								.Select(um => um.Id)
								.ToHashSet();

			MarkSelectedMenus(Menus, userMenuIds);

			// Subscribe untuk auto-save saat checkbox berubah
			SubscribeAutoSave(Menus);
		}

		// ===== Tandai menu sesuai user =====
		private void MarkSelectedMenus(ObservableCollection<MenuCheckItem> menus, HashSet<int> userMenuIds)
		{
			foreach (var menu in menus)
			{
				menu.IsSelected = userMenuIds.Contains(menu.Id);
				if (menu.Children.Any())
					MarkSelectedMenus(menu.Children, userMenuIds);
			}
		}

		// ===== Ambil semua menu yang terpilih =====
		private List<int> GetSelectedMenuIds(ObservableCollection<MenuCheckItem> menus)
		{
			var list = new List<int>();
			foreach (var menu in menus)
			{
				if (menu.IsSelected)
					list.Add(menu.Id);

				if (menu.Children.Any())
					list.AddRange(GetSelectedMenuIds(menu.Children));
			}
			return list;
		}

		// ===== Auto-save saat checkbox berubah =====
		private void SubscribeAutoSave(ObservableCollection<MenuCheckItem> menus)
		{
			foreach (var menu in menus)
			{
				menu.PropertyChanged += async (s, e) =>
				{
					if (e.PropertyName == nameof(MenuCheckItem.IsSelected) && SelectedUser != null)
					{
						var selectedIds = GetSelectedMenuIds(Menus);
						await _userService.SetUserMenusAsync(SelectedUser.Id, selectedIds);
					}
				};

				if (menu.Children.Any())
					SubscribeAutoSave(menu.Children);
			}
		}

		// ===== Auto-load saat SelectedUser berubah =====
		partial void OnSelectedUserChanged(User? oldValue, User? newValue)
		{
			if (newValue != null)
			{
				_ = UserSelected();
			}
		}
	}
}
