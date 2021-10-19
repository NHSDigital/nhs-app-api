using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Areas.Errors.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FullNavigationTryAgainFileDownloadErrorPage : IFullNavigationTryAgainFileDownloadErrorView, IFullNavigationTryAgainFileDownloadErrorView.IEvents
    {
        private readonly ILogger<FullNavigationTryAgainFileDownloadErrorPage> _logger;
        private readonly AppNavigation<IFullNavigationTryAgainFileDownloadErrorView.IEvents> _appNavigation;

        public FullNavigationTryAgainFileDownloadErrorPage(ILogger<FullNavigationTryAgainFileDownloadErrorPage> logger, IAccessibilityService accessibilityService): base(accessibilityService)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<IFullNavigationTryAgainFileDownloadErrorView.IEvents>(this, Navigation);

            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
        }

        IAppNavigation<IFullNavigationTryAgainFileDownloadErrorView.IEvents> INavigationView<IFullNavigationTryAgainFileDownloadErrorView.IEvents>.AppNavigation => _appNavigation;

        public void SetNavigationFooterItem(NavigationFooterItem footerItem) => SelectedNavigationFooterItem = footerItem;

        public Func<Task>? TryAgainRequested { get; set; }
        public ICommand TryAgainCommand => new AsyncCommand(() => TryAgainRequested);

        protected override void OnAppearing()
        {
            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            _logger.LogInformation("{Method}", nameof(OnDisappearing));
            _appNavigation.SuppressHandlers();
        }

        public Task HandleDeeplink(Uri deeplinkUrl)
        {
            _logger.LogInformation("{ClassName} is not required to handle deeplinks", nameof(IFullNavigationTryAgainFileDownloadErrorView));
            return Task.CompletedTask;
        }
    }
}