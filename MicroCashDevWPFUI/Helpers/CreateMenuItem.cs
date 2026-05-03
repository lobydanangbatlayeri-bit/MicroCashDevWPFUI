using MicroCashDevWPFUI.Models;
using Wpf.Ui.Controls;
using System;
using System.Linq;

namespace MicroCashDevWPFUI.Helpers
{
	public static class MenuHelper
	{
		public static NavigationViewItem CreateMenuItem(Menu menu)
		{
			var pageType = Type.GetType($"MicroCashDevWPFUI.Views.Pages.{menu.PageKey}");

			var item = new NavigationViewItem
			{
				Content = menu.NamaMenu,
				TargetPageType = pageType
			};

			if (!string.IsNullOrEmpty(menu.Icon) && Enum.TryParse<SymbolRegular>(menu.Icon, out var symbol))
				item.Icon = new SymbolIcon { Symbol = symbol };
			else
				item.Icon = new SymbolIcon { Symbol = SymbolRegular.Notebook24 };

			if (menu.Children != null && menu.Children.Any())
			{
				foreach (var child in menu.Children.OrderBy(c => c.Urutan))
					item.MenuItems.Add(CreateMenuItem(child));
			}

			return item;
		}
	}
}
