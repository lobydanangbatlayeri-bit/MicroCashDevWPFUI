using CommunityToolkit.Mvvm.ComponentModel;
using MicroCashDevWPFUI.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MicroCashDevWPFUI.DTOs
{
	public partial class MenuCheckItem : ObservableObject
	{
		public int Id { get; set; }
		public string NamaMenu { get; set; } = string.Empty;

		[ObservableProperty]
		private bool isSelected;

		public ObservableCollection<MenuCheckItem> Children { get; set; } = new();

		public MenuCheckItem? Parent { get; set; } = null; // parent reference

		partial void OnIsSelectedChanged(bool oldValue, bool newValue)
		{
			// ===== Parent-child auto check =====
			if (newValue)
			{
				// jika dicentang, pastikan parent juga tercentang
				Parent?.SetIsSelectedRecursive(true, false);
			}
			else
			{
				// jika di-uncheck, uncheck semua child
				foreach (var child in Children)
				{
					child.SetIsSelectedRecursive(false, true);
				}
			}
		}

		// ===== Recursive set without infinite loop =====
		public void SetIsSelectedRecursive(bool value, bool updateChildren)
		{
			if (IsSelected == value) return;

			IsSelected = value;

			if (updateChildren)
			{
				foreach (var child in Children)
				{
					child.SetIsSelectedRecursive(value, true);
				}
			}
		}

		// ===== Buat dari model Menu dan set parent =====
		public static MenuCheckItem FromMenuModel(Menu menu, MenuCheckItem? parent = null)
		{
			var item = new MenuCheckItem
			{
				Id = menu.Id,
				NamaMenu = menu.NamaMenu,
				Parent = parent
			};

			item.Children = new ObservableCollection<MenuCheckItem>(
				menu.Children
					.OrderBy(c => c.Urutan)
					.Select(c => FromMenuModel(c, item))
			);

			return item;
		}
	}
}