using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.DependencyServices.Navigation;
using NHSOnline.App.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Areas.Errors.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PreHomeTryAgainNetworkErrorPage : IPreHomeTryAgainNetworkErrorView, IPreHomeTryAgainNetworkErrorView.IEvents, ISwipeablePage
    {
        private readonly ILogger<PreHomeTryAgainNetworkErrorPage> _logger;
        private readonly AppNavigation<IPreHomeTryAgainNetworkErrorView.IEvents> _appNavigation;
        private readonly INavigationService _navigationService;

        public PreHomeTryAgainNetworkErrorPage(ILogger<PreHomeTryAgainNetworkErrorPage> logger, INavigationService navigationService)
        {
            _logger = logger;
            _navigationService = navigationService;
            _appNavigation = new AppNavigation<IPreHomeTryAgainNetworkErrorView.IEvents>(this, _navigationService);

            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
        }

        IAppNavigation<IPreHomeTryAgainNetworkErrorView.IEvents> INavigationView<IPreHomeTryAgainNetworkErrorView.IEvents>.AppNavigation => _appNavigation;

        public Func<Task>? OneOneOneRequested { get; set; }
        public ICommand OneOneOneCommand => new AsyncCommand(() => OneOneOneRequested);

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

        public bool OnSwipeBack()
        {
            BackRequestedCommand.Execute(null);
            return true;
        }

        public Task HandleDeeplink(Uri deeplinkUrl)
        {
            _logger.LogInformation("{ClassName} is not required to handle deeplinks", nameof(PreHomeTryAgainNetworkErrorPage));
            return Task.CompletedTask;
        }
    }
}