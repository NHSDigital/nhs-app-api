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
    public partial class FullNavigationBackToHomeNetworkErrorPage : IFullNavigationBackToHomeNetworkErrorView, IFullNavigationBackToHomeNetworkErrorView.IEvents
    {
        private readonly ILogger<FullNavigationBackToHomeNetworkErrorPage> _logger;
        private readonly AppNavigation<IFullNavigationBackToHomeNetworkErrorView.IEvents> _appNavigation;

        public FullNavigationBackToHomeNetworkErrorPage(ILogger<FullNavigationBackToHomeNetworkErrorPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<IFullNavigationBackToHomeNetworkErrorView.IEvents>(this, Navigation);

            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
        }

        IAppNavigation<IFullNavigationBackToHomeNetworkErrorView.IEvents> INavigationView<IFullNavigationBackToHomeNetworkErrorView.IEvents>.AppNavigation => _appNavigation;

        public void SetNavigationFooterItem(NavigationFooterItem footerItem) => SelectedNavigationFooterItem = footerItem;

        public Func<Task>? BackToHomeRequested { get; set; }
        public ICommand BackToHomeCommand => new AsyncCommand(() => BackToHomeRequested);

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
            _logger.LogInformation("{ClassName} is not required to handle deeplinks", nameof(FullNavigationBackToHomeNetworkErrorPage));
            return Task.CompletedTask;
        }
    }
}