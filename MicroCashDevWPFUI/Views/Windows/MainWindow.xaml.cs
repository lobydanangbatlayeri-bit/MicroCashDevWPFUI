using CommunityToolkit.Mvvm.Messaging;
using MicroCashDevWPFUI.Messages;
using MicroCashDevWPFUI.ViewModels.Windows;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace MicroCashDevWPFUI.Views.Windows
{
    public partial class MainWindow : INavigationWindow
    {
        public MainWindowViewModel ViewModel { get; }
		private readonly IContentDialogService _dialogService;

		public MainWindow(
            MainWindowViewModel viewModel,
            INavigationViewPageProvider navigationViewPageProvider,
            INavigationService navigationService,
            IContentDialogService contentDialogService
		)
        {
            ViewModel = viewModel;
            DataContext = this;

            SystemThemeWatcher.Watch(this);

            InitializeComponent();
			_dialogService = contentDialogService;
			SetPageService(navigationViewPageProvider);

            navigationService.SetNavigationControl(RootNavigation);

			WeakReferenceMessenger.Default.Register<LogoutMessage>(this, (_, _) =>
			{
				var loginWindow = App.GetService<LoginWindow>();
				loginWindow.Show();
				this.Hide();
			});
		}

		#region INavigationWindow methods

		public INavigationView GetNavigation() => RootNavigation;

        public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

        public void SetPageService(INavigationViewPageProvider navigationViewPageProvider) => RootNavigation.SetPageProviderService(navigationViewPageProvider);

        public void ShowWindow() => Show();

        public void CloseWindow() => Close();

        #endregion INavigationWindow methods

        /// <summary>
        /// Raises the closed event.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Make sure that closing this window will begin the process of closing the application.
            Application.Current.Shutdown();
        }

        INavigationView INavigationWindow.GetNavigation()
        {
            throw new NotImplementedException();
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			_dialogService.SetDialogHost(RootContentDialog);
		}

	}
}
