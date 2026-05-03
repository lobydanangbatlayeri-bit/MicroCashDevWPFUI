using Wpf.Ui.Controls;

namespace MicroCashDevWPFUI.Helpers
{
	public static class PasswordBoxHelper
	{
		private static bool _isUpdating = false;

		public static readonly DependencyProperty BoundPasswordProperty =
			DependencyProperty.RegisterAttached(
				"BoundPassword",
				typeof(string),
				typeof(PasswordBoxHelper),
				new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundPasswordChanged));

		public static string GetBoundPassword(DependencyObject obj)
			=> (string)obj.GetValue(BoundPasswordProperty);

		public static void SetBoundPassword(DependencyObject obj, string value)
			=> obj.SetValue(BoundPasswordProperty, value);

		public static readonly DependencyProperty BindPasswordProperty =
			DependencyProperty.RegisterAttached(
				"BindPassword",
				typeof(bool),
				typeof(PasswordBoxHelper),
				new PropertyMetadata(false, OnBindPasswordChanged));

		public static bool GetBindPassword(DependencyObject obj)
			=> (bool)obj.GetValue(BindPasswordProperty);

		public static void SetBindPassword(DependencyObject obj, bool value)
			=> obj.SetValue(BindPasswordProperty, value);

		private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is PasswordBox box)
			{
				box.PasswordChanged -= PasswordChanged;

				if (!_isUpdating)
					box.Password = e.NewValue as string ?? string.Empty;

				box.PasswordChanged += PasswordChanged;
			}
		}

		private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
		{
			if (dp is PasswordBox box)
			{
				bool wasBound = (bool)e.OldValue;
				bool needToBind = (bool)e.NewValue;

				if (wasBound)
					box.PasswordChanged -= PasswordChanged;

				if (needToBind)
					box.PasswordChanged += PasswordChanged;
			}
		}

		private static void PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (_isUpdating) return;

			if (sender is PasswordBox box)
			{
				_isUpdating = true;
				SetBoundPassword(box, box.Password);
				_isUpdating = false;
			}
		}
	}
}
