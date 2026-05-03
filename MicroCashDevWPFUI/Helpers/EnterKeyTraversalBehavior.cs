using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MicroCashDevWPFUI.Helpers
{
	public static class EnterKeyTraversalBehavior
	{
		public static void Register()
		{
			InputManager.Current.PreProcessInput += OnPreProcessInput;
		}

		private static void OnPreProcessInput(object sender, PreProcessInputEventArgs e)
		{
			if (e.StagingItem.Input is not KeyEventArgs keyArgs)
				return;

			if (keyArgs.RoutedEvent != Keyboard.KeyDownEvent)
				return;

			if (keyArgs.Key != Key.Enter)
				return;

			// Skip multiline textbox
			if (Keyboard.FocusedElement is TextBox tb && tb.AcceptsReturn)
				return;

			// Skip DataGrid editing
			if (Keyboard.FocusedElement is DataGrid)
				return;

			keyArgs.Handled = true;

			if (Keyboard.FocusedElement is UIElement element)
			{
				element.MoveFocus(
					new TraversalRequest(FocusNavigationDirection.Next));
			}
		}
	}
}