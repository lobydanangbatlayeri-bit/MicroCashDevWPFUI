using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace MicroCashDevWPFUI.Services.CoreService
{
	public interface IDialogService
	{
		Task ShowMessage(string message);
		Task<ContentDialogResult> ShowConfirmation(
		string title,
		string message,
		string primaryText = "OK",
		string closeText = "Tidak");
		Task<ContentDialogResult> ShowContentDialogAsync(
			string title,
			UserControl content,
			string primaryText = "OK",
			string closeText = "Close");
	}
}
