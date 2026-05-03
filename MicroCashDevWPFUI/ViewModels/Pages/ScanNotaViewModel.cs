using MicroCashDevWPFUI.Api;
using QRCoder;
using System.IO;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;

namespace MicroCashDevWPFUI.ViewModels.Pages
{
	public partial class ScanNotaViewModel : ObservableObject
	{
		private readonly IApiHostService _apiHostService;

		public ScanNotaViewModel(IApiHostService apiHostService)
		{
			_apiHostService = apiHostService;

			_ = InitializeAsync();
		}

		// ================= PROPERTIES =================

		[ObservableProperty]
		private string serverUrl = string.Empty;

		[ObservableProperty]
		private string serverStatus = "Mendeteksi IP...";

		[ObservableProperty]
		private BitmapImage? qrCodeImage;

		[ObservableProperty]
		private InfoBarSeverity serverSeverity = InfoBarSeverity.Informational;

		[ObservableProperty]
		private string serverTitle = "Mendeteksi Server...";

		[ObservableProperty]
		private bool isLoading;

		// ================= METHOD =================
		private async Task InitializeAsync()
		{
			try
			{
				await _apiHostService.StartAsync();
				RefreshIp();
			}
			catch (Exception ex)
			{
				ServerSeverity = InfoBarSeverity.Error;
				ServerTitle = "Server Gagal Start";
				ServerStatus = ex.Message;
			}
		}

		// ================= COMMANDS =================

		[RelayCommand]
		private void RefreshIp()
		{
			if (!_apiHostService.IsRunning ||
				string.IsNullOrEmpty(_apiHostService.CurrentIp))
			{
				ServerStatus = "Server belum aktif.";
				ServerSeverity = InfoBarSeverity.Error;
				ServerTitle = "Server Tidak Aktif";
				return;
			}

			ServerUrl = $"http://{_apiHostService.CurrentIp}:{_apiHostService.Port}";
			ServerStatus = "Siap menerima scan dari HP.";
			ServerSeverity = InfoBarSeverity.Success;
			ServerTitle = "Server Aktif";

			GenerateQrCode(ServerUrl);
		}

		[RelayCommand]
		private void CopyUrl()
		{
			if (!string.IsNullOrWhiteSpace(ServerUrl))
			{
				Clipboard.SetText(ServerUrl);
			}
		}

		// ================= HELPERS =================

		private void GenerateQrCode(string url)
		{
			var qrGenerator = new QRCodeGenerator();
			var qrData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
			var qrCode = new PngByteQRCode(qrData);

			byte[] qrBytes = qrCode.GetGraphic(20);

			using var ms = new MemoryStream(qrBytes);

			var image = new BitmapImage();
			image.BeginInit();
			image.StreamSource = ms;
			image.CacheOption = BitmapCacheOption.OnLoad;
			image.EndInit();
			image.Freeze();

			QrCodeImage = image;
		}
	}
}
