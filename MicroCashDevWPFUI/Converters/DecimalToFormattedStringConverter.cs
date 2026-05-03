using System;
using System.Globalization;
using System.Windows.Data;

namespace MicroCashDevWPFUI.Converters
{
	public class DecimalToFormattedStringConverter : IValueConverter
	{
		private static readonly CultureInfo IdCulture = CultureInfo.GetCultureInfo("id-ID");

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return string.Empty;
			if (value is decimal dec)
				return dec.ToString("N2", IdCulture); // always show 2 decimals
			if (value is double d)
				return ((decimal)d).ToString("N2", IdCulture);
			if (value is int i)
				return ((decimal)i).ToString("N2", IdCulture);
			if (decimal.TryParse(value.ToString(), out var parsed))
				return parsed.ToString("N2", IdCulture);
			return string.Empty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var s = (value as string) ?? string.Empty;
			if (string.IsNullOrWhiteSpace(s)) return 0m;

			// Remove thousand separators and normalize decimal separator
			var cleaned = s.Replace(".", "").Replace(",", ".");
			if (decimal.TryParse(cleaned, System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
				return result;

			return 0m;
		}
	}
}