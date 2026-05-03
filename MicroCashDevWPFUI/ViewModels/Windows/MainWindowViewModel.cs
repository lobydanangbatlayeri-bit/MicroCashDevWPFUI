using CommunityToolkit.Mvvm.Messaging;
using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Helpers;
using MicroCashDevWPFUI.Messages;
using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.CoreService;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace MicroCashDevWPFUI.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
		private readonly IMenuService _menuService;
		private readonly IUserSessionService _session;
		private readonly INotificationService _notificationService;

		[ObservableProperty]
        private string _applicationTitle = "MicroCash";

		[ObservableProperty]
		private User? currentUser;

		[ObservableProperty]
		private ObservableCollection<object> _menuItems = new();

		[ObservableProperty]
		private Page? _currentPage;

		[ObservableProperty]
		private bool _isFlyoutOpen = false;

		[ObservableProperty]
		private ObservableCollection<AppNotification> _notifications = new();

		public MainWindowViewModel(IMenuService menuService, IUserSessionService sessionService, INotificationService notificationService)
		{
			_menuService = menuService;
			_session = sessionService;
			_notificationService = notificationService;

			CurrentUser = _session.CurrentUser;

			WeakReferenceMessenger.Default.Register<LoginSuccessMessage>(this, (r, m) =>
			{
				CurrentUser = m.Value;
				_ = LoadMenuAsync(CurrentUser.Id);
			});
		}

		public async Task LoadMenuAsync(int userId)
		{
			var menus = await _menuService.GetMenusForUserAsync(userId);
			MenuItems.Clear();
			foreach (var menu in menus)
			{
				MenuItems.Add(MenuHelper.CreateMenuItem(menu));
			}
		}

		public async Task LoadNotificationsAsync()
		{
			var data = await _notificationService.GetNotificationsAsync();

			Notifications.Clear();

			foreach (var item in data)
			{
				Notifications.Add(item);
			}
		}

		[RelayCommand]
		private async Task Notifikasi()
		{
			await LoadNotificationsAsync();
			IsFlyoutOpen = !IsFlyoutOpen;
		}

		[RelayCommand]
		private void Logout()
		{
			_session.Logout();

			CurrentUser = null;
			MenuItems.Clear();
			CurrentPage = null;

			WeakReferenceMessenger.Default.Send(new LogoutMessage());
		}
	}
}
