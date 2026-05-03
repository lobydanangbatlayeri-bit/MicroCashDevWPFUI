using MicroCashDevWPFUI.DTOs.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MicroCashDevWPFUI.Helpers
{
	public static class DataGridAutoSaveBehavior
	{
		public static bool GetIsEnabled(DependencyObject obj)
			=> (bool)obj.GetValue(IsEnabledProperty);

		public static void SetIsEnabled(DependencyObject obj, bool value)
			=> obj.SetValue(IsEnabledProperty, value);

		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.RegisterAttached(
				"IsEnabled",
				typeof(bool),
				typeof(DataGridAutoSaveBehavior),
				new PropertyMetadata(false, OnChanged));

		private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is not DataGrid grid)
				return;

			if ((bool)e.NewValue)
				grid.RowEditEnding += OnRowEditEnding;
			else
				grid.RowEditEnding -= OnRowEditEnding;
		}

		private static async void OnRowEditEnding(object? sender, DataGridRowEditEndingEventArgs e)
		{
			if (e.EditAction != DataGridEditAction.Commit)
				return;

			if (e.Row.Item is IAutoSaveItem item)
				await item.SaveAsync();
		}
	}

}
