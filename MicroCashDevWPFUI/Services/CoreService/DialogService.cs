using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace MicroCashDevWPFUI.Services.CoreService
{
	public class DialogService : IDialogService
	{
		private readonly IContentDialogService _contentDialogService;

		public DialogService(IContentDialogService contentDialogService)
		{
			_contentDialogService = contentDialogService;
		}

		public async Task ShowMessage(string message)
		{
			await _contentDialogService.ShowAsync(new ContentDialog
			{
				Title = "Informasi",
				Content = message,
				PrimaryButtonText = "OK"
			}, CancellationToken.None);
		}

		public async Task<ContentDialogResult> ShowConfirmation(
				string title,
				string message,
				string primaryText = "OK",
				string closeText = "Tidak")
		{
			return await _contentDialogService.ShowAsync(new ContentDialog
			{
				Title = title,
				Content = message,
				PrimaryButtonText = primaryText,
				CloseButtonText = closeText,
				DefaultButton = ContentDialogButton.Primary
			}, CancellationToken.None);
		}


		public async Task<ContentDialogResult> ShowContentDialogAsync(
			string title,
			UserControl content,
			string primaryText = "OK",
			string closeText = "Close")
		{
			return await _contentDialogService.ShowAsync(new ContentDialog
			{
				Title = title,
				Content = content,
				PrimaryButtonText = primaryText,
				CloseButtonText = closeText,
				DefaultButton = ContentDialogButton.Primary
			}, CancellationToken.None);
		}
	}
}