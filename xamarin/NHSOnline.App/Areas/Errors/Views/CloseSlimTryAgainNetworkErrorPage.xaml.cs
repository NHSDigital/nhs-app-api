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
    public partial class CloseSlimTryAgainNetworkErrorPage : ICloseSlimTryAgainNetworkErrorView, ICloseSlimTryAgainNetworkErrorView.IEvents, ISwipeablePage
    {
        private readonly ILogger<CloseSlimTryAgainNetworkErrorPage> _logger;
        private readonly AppNavigation<ICloseSlimTryAgainNetworkErrorView.IEvents> _appNavigation;

        public CloseSlimTryAgainNetworkErrorPage(ILogger<CloseSlimTryAgainNetworkErrorPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<ICloseSlimTryAgainNetworkErrorView.IEvents>(this, Navigation);

            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
        }

        IAppNavigation<ICloseSlimTryAgainNetworkErrorView.IEvents> INavigationView<ICloseSlimTryAgainNetworkErrorView.IEvents>.AppNavigation => _appNavigation;

        public Func<Task>? TryAgainRequested { get; set; }
        public ICommand TryAgainCommand => new AsyncCommand(() => TryAgainRequested);

        public Func<Task>? BackRequested { get; set; }
        private ICommand BackRequestedCommand => new AsyncCommand(() => BackRequested);

        public Func<Task>? CloseRequested { get; set; }
        public ICommand CloseRequestedCommand => new AsyncCommand(() => CloseRequested);

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

        public bool OnSwipeBack()
        {
            BackRequestedCommand.Execute(null);
            return true;
        }

        public Task HandleDeeplink(Uri deeplinkUrl)
        {
            _logger.LogInformation("{ClassName} is not required to handle deeplinks", nameof(CloseSlimTryAgainNetworkErrorPage));
            return Task.CompletedTask;
        }
    }
}