using MicroCashDevWPFUI.Views.Pages;
using MicroCashDevWPFUI.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;

namespace MicroCashDevWPFUI.Services.CoreService
{
    /// <summary>
    /// Managed host of the application.
    /// </summary>
    public class ApplicationHostService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public ApplicationHostService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await HandleActivationAsync();
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

		/// <summary>
		/// Creates main window during activation.
		/// </summary>
		private async Task HandleActivationAsync()
		{
			if (!Application.Current.Windows.OfType<LoginWindow>().Any())
			{
				var loginWindow = _serviceProvider.GetService<LoginWindow>()!;
				loginWindow.Show();
			}

			await Task.CompletedTask;
		}

	}
}
