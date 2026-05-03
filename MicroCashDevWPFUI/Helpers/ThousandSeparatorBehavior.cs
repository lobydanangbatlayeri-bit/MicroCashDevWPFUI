using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MicroCashDevWPFUI.Helpers
{
	public static class ThousandSeparatorBehavior
	{
		private static readonly ConcurrentDictionary<TextBox, bool> _isUpdating = new();
		// allow digits and comma/dot
		private static readonly Regex _allowedChars = new(@"^[0-9\.,]+$");

		public static bool GetIsEnabled(DependencyObject obj) => (bool)obj.GetValue(IsEnabledProperty);
		public static void SetIsEnabled(DependencyObject obj, bool value) => obj.SetValue(IsEnabledProperty, value);

		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.RegisterAttached(
				"IsEnabled",
				typeof(bool),
				typeof(ThousandSeparatorBehavior),
				new PropertyMetadata(false, OnIsEnabledChanged));

		private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is not TextBox tb) return;
			if ((bool)e.NewValue)
			{
				tb.PreviewTextInput += Tb_PreviewTextInput;
				DataObject.AddPastingHandler(tb, OnPaste);
				tb.TextChanged += Tb_TextChanged;
			}
			else
			{
				tb.PreviewTextInput -= Tb_PreviewTextInput;
				DataObject.RemovePastingHandler(tb, OnPaste);
				tb.TextChanged -= Tb_TextChanged;
			}
		}

		private static void Tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !_allowedChars.IsMatch(e.Text);
		}

		private static void OnPaste(object sender, DataObjectPastingEventArgs e)
		{
			if (e.DataObject.GetDataPresent(DataFormats.Text))
			{
				var text = e.DataObject.GetData(DataFormats.Text) as string ?? string.Empty;
				if (!Regex.IsMatch(text, @"^[0-9\.,\s]+$"))
					e.CancelCommand();
			}
			else e.CancelCommand();
		}

		private static void Tb_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is not TextBox tb) return;
			if (_isUpdating.TryGetValue(tb, out var updating) && updating) return;

			try
			{
				_isUpdating[tb] = true;

				int selStart = tb.SelectionStart;
				string oldText = tb.Text ?? string.Empty;

				// Count digits before caret (all digits)
				int digitsBeforeCaret = oldText.Substring(0, Math.Min(selStart, oldText.Length))
					.Count(char.IsDigit);

				// Determine separator position (prefer comma as decimal if present)
				int sepIndex = oldText.IndexOf(',');
				if (sepIndex == -1) sepIndex = oldText.IndexOf('.');

				// Extract integer and fractional digits
				string integerDigits, fracDigits;
				if (sepIndex >= 0)
				{
					integerDigits = string.Concat(oldText.Substring(0, sepIndex).Where(char.IsDigit));
					fracDigits = string.Concat(oldText.Substring(sepIndex + 1).Where(char.IsDigit));
				}
				else
				{
					integerDigits = string.Concat(oldText.Where(char.IsDigit));
					fracDigits = string.Empty;
				}

				// limit fractional digits to 2
				if (fracDigits.Length > 2) fracDigits = fracDigits.Substring(0, 2);

				if (string.IsNullOrEmpty(integerDigits))
				{
					// if only fractional typed like ",5" -> integer 0
					integerDigits = "0";
				}

				// parse integer part safely
				if (!long.TryParse(integerDigits, out var intValue))
				{
					intValue = 0;
				}

				// format integer part with thousand separator (id-ID)
				string formattedInt = intValue.ToString("N0", CultureInfo.GetCultureInfo("id-ID"));

				// build new text: include fractional part if user typed separator or there is fractional digits
				string newText;
				// determine if user has typed a separator (present in oldText)
				bool hasSep = (oldText.IndexOf(',') >= 0 || oldText.IndexOf('.') >= 0);
				if (hasSep || fracDigits.Length > 0)
				{
					newText = formattedInt + "," + fracDigits;
				}
				else
				{
					newText = formattedInt;
				}

				// compute new caret position based on digitsBeforeCaret
				int newCaret = GetCaretFromDigitsCount(newText, digitsBeforeCaret);

				// apply
				if (tb.Text != newText)
				{
					tb.Text = newText;
				}
				// ensure caret within bounds
				tb.SelectionStart = Math.Min(newCaret, tb.Text.Length);
			}
			finally
			{
				_isUpdating[tb] = false;
			}
		}

		private static int GetCaretFromDigitsCount(string formatted, int digitsBefore)
		{
			if (digitsBefore <= 0) return 0;
			int count = 0;
			for (int i = 0; i < formatted.Length; i++)
			{
				if (char.IsDigit(formatted[i]))
				{
					count++;
					if (count == digitsBefore)
						return i + 1;
				}
			}
			return formatted.Length;
		}
	}
}