using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MicroCashDevWPFUI.Converters
{
	public class BoolToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool b)
			{
				if (parameter is string param && param == "Invert")
					b = !b;

				return b ? Visibility.Visible : Visibility.Collapsed;
			}
			return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Visibility v)
			{
				bool result = v == Visibility.Visible;
				if (parameter is string param && param == "Invert")
					result = !result;
				return result;
			}
			return false;
		}
	}
}
