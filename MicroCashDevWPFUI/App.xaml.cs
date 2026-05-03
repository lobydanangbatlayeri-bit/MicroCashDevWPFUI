using Autofac;
using Autofac.Extensions.DependencyInjection;
using MicroCashDevWPFUI.Data;
using MicroCashDevWPFUI.Services.CoreService;
using MicroCashDevWPFUI.ViewModels.Pages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.DependencyInjection;

namespace MicroCashDevWPFUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
		// The.NET Generic Host provides dependency injection, configuration, logging, and other services.
		// https://docs.microsoft.com/dotnet/core/extensions/generic-host
		// https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
		// https://docs.microsoft.com/dotnet/core/extensions/configuration
		// https://docs.microsoft.com/dotnet/core/extensions/logging		
		private static readonly IHost _host = Host
			.CreateDefaultBuilder()
			.UseServiceProviderFactory(new AutofacServiceProviderFactory())
			.ConfigureContainer<ContainerBuilder>(builder =>
			{
				// Registrasi Dynamic seperti project kamu sebelumnya
				builder.RegisterType<AppDbContext>()
					.As<IAppDbContext>()
					.InstancePerLifetimeScope();

				builder.RegisterAssemblyTypes(typeof(App).Assembly)
					.Where(t => t.Name.EndsWith("Service"))
					.AsImplementedInterfaces()
					.InstancePerLifetimeScope();

				builder.RegisterAssemblyTypes(typeof(App).Assembly)
					.Where(t => t.Name.EndsWith("Matcher"))
					.AsImplementedInterfaces()
					.InstancePerLifetimeScope();

				builder.RegisterType<RestokNotaViewModel>()
					.SingleInstance();

				builder.RegisterType<RestokManualViewModel>()
					.SingleInstance();

				builder.RegisterAssemblyTypes(typeof(App).Assembly)
					.Where(t => t.Name.EndsWith("ViewModel"))
					.Except<RestokNotaViewModel>()
					.Except<RestokManualViewModel>()
					.InstancePerDependency();

				builder.RegisterAssemblyTypes(typeof(App).Assembly)
					.Where(t => t.Name.EndsWith("Page"))
					.AsSelf()
					.InstancePerDependency();

				builder.RegisterAssemblyTypes(typeof(App).Assembly)
					.Where(t => t.Name.EndsWith("Window"))
					.AsSelf()
					.SingleInstance();

				builder.RegisterAssemblyTypes(typeof(App).Assembly)
					.Where(t => typeof(INavigationWindow).IsAssignableFrom(t) && !t.IsInterface)
					.As<INavigationWindow>()
					.SingleInstance();
			})
			.ConfigureServices((context, services) =>
			{
				services.AddNavigationViewPageProvider();

				services.AddHostedService<ApplicationHostService>();

				services.AddSingleton<IThemeService, ThemeService>();
				services.AddSingleton<ITaskBarService, TaskBarService>();
				services.AddSingleton<INavigationService, NavigationService>();
				services.AddSingleton<IContentDialogService, ContentDialogService>();
				services.AddSingleton<IDialogService, DialogService>();
			})
			.Build();


		/// <summary>
		/// Gets services.
		/// </summary>
		public static IServiceProvider Services
        {
            get { return _host.Services; }
        }

		public static T GetService<T>() where T : class
		{
			return Services.GetService(typeof(T)) as T
				?? throw new InvalidOperationException($"Unable to resolve type {typeof(T)}");
		}

		/// <summary>
		/// Occurs when the application is loading.
		/// </summary>
		private async void OnStartup(object sender, StartupEventArgs e)
		{
			try
			{
				await _host.StartAsync();

				using (var scope = _host.Services.CreateScope())
				{
					var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
					if (db is AppDbContext context)
					{
						context.Database.Migrate();
						DbSeed.Initialize(context);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Startup Error");
			}
		}

		/// <summary>
		/// Occurs when the application is closing.
		/// </summary>
		private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();

            _host.Dispose();
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
        }
    }
}
