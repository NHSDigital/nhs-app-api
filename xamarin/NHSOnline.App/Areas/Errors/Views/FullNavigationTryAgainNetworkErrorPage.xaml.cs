using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Areas.Errors.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FullNavigationTryAgainNetworkErrorPage : IFullNavigationTryAgainNetworkErrorView, IFullNavigationTryAgainNetworkErrorView.IEvents
    {
        private readonly ILogger<FullNavigationTryAgainNetworkErrorPage> _logger;
        private readonly AppNavigation<IFullNavigationTryAgainNetworkErrorView.IEvents> _appNavigation;

        public FullNavigationTryAgainNetworkErrorPage(ILogger<FullNavigationTryAgainNetworkErrorPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<IFullNavigationTryAgainNetworkErrorView.IEvents>(this, Navigation);

            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
        }

        IAppNavigation<IFullNavigationTryAgainNetworkErrorView.IEvents> INavigationView<IFullNavigationTryAgainNetworkErrorView.IEvents>.AppNavigation => _appNavigation;

        public void SetNavigationFooterItem(NavigationFooterItem footerItem) => SelectedNavigationFooterItem = footerItem;

        public Func<Task>? TryAgainRequested { get; set; }
        public ICommand TryAgainCommand => new AsyncCommand(() => TryAgainRequested);

        public Func<Task>? BackRequested { get; set; }
        private ICommand BackRequestedCommand => new AsyncCommand(() => BackRequested);

        protected override void OnAppearing()
        {
            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();

            ErrorPanel.AccessibilityFocusHeading();
        }

        protected override void OnDisappearing()
        {
            _logger.LogInformation("{Method}", nameof(OnDisappearing));
            _appNavigation.SuppressHandlers();
        }

        protected override bool OnBackButtonPressed()
        {
            BackRequestedCommand.Execute(null);
            return true;
        }

        public Task HandleDeeplink(Uri deeplinkUrl)
        {
            _logger.LogInformation("{ClassName} is not required to handle deeplinks", nameof(FullNavigationTryAgainNetworkErrorPage));
            return Task.CompletedTask;
        }
    }
}