using MicroCashDevWPFUI.Api.Endpoints;
using MicroCashDevWPFUI.Api.Infrastructure;
using MicroCashDevWPFUI.OCR.Engine;
using MicroCashDevWPFUI.OCR.Engine.Interfaces;
using MicroCashDevWPFUI.OCR.Processing;
using MicroCashDevWPFUI.OCR.Processing.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace MicroCashDevWPFUI.Api
{
	public class ApiHostService : IApiHostService
	{
		private readonly ScanNotaMemoryStore _store;

		private WebApplication? _app;

		public bool IsRunning => _app != null;

		public string? CurrentIp { get; private set; }
		public int Port => 5000;

		public ApiHostService(ScanNotaMemoryStore store)
		{
			_store = store;
		}

		public async Task StartAsync()
		{
			if (_app != null)
				return;

			var ip = GetLocalIpAddress();
			if (ip == null)
				throw new Exception("IP lokal tidak ditemukan.");

			CurrentIp = ip;

			var options = new WebApplicationOptions
			{
				WebRootPath = Path.Combine(AppContext.BaseDirectory, "Api", "Web"),
				ContentRootPath = AppContext.BaseDirectory
			};

			var builder = WebApplication.CreateBuilder(options);

			builder.WebHost.ConfigureKestrel(serverOptions =>
			{
				serverOptions.ListenAnyIP(Port);
			});

			builder.Services.AddRouting();
			builder.Services.AddSingleton(_store);
			builder.Services.AddSingleton<IOcrService, TesseractOcrService>();

			_app = builder.Build();

			_app.UseDefaultFiles();
			_app.UseStaticFiles();

			MapEndpoints(_app);

			await _app.StartAsync();
		}

		public async Task StopAsync()
		{
			if (_app == null)
				return;

			await _app.StopAsync();
			_app = null;
			CurrentIp = null;
		}

		private static void MapEndpoints(WebApplication app)
		{
			app.MapScanNotaEndpoints();
		}

		private string? GetLocalIpAddress()
		{
			foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (ni.OperationalStatus != OperationalStatus.Up)
					continue;

				if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback)
					continue;

				var props = ni.GetIPProperties();

				foreach (var addr in props.UnicastAddresses)
				{
					if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
						return addr.Address.ToString();
				}
			}

			return null;
		}
	}
}