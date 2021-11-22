using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.DependencyServices.Navigation;
using NHSOnline.App.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Areas.Errors.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CloseSlimBackToHomeNetworkErrorPage : ICloseSlimBackToHomeNetworkErrorView, ICloseSlimBackToHomeNetworkErrorView.IEvents, ISwipeablePage
    {
        private readonly ILogger<CloseSlimBackToHomeNetworkErrorPage> _logger;
        private readonly AppNavigation<ICloseSlimBackToHomeNetworkErrorView.IEvents> _appNavigation;
        private readonly INavigationService _navigationService;

        public CloseSlimBackToHomeNetworkErrorPage(ILogger<CloseSlimBackToHomeNetworkErrorPage> logger, IAccessibilityService accessibilityService, INavigationService navigationService): base(accessibilityService)
        {
            _logger = logger;
            _navigationService = navigationService;
            _appNavigation = new AppNavigation<ICloseSlimBackToHomeNetworkErrorView.IEvents>(this, _navigationService);

            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
        }

        IAppNavigation<ICloseSlimBackToHomeNetworkErrorView.IEvents> INavigationView<ICloseSlimBackToHomeNetworkErrorView.IEvents>.AppNavigation => _appNavigation;

        public Func<Task>? OneOneOneRequested { get; set; }
        public ICommand OneOneOneCommand => new AsyncCommand(() => OneOneOneRequested);

        public Func<Task>? BackToHomeRequested { get; set; }
        public ICommand BackToHomeCommand => new AsyncCommand(() => BackToHomeRequested);

        public Func<Task>? BackRequested { get; set; }
        private ICommand BackRequestedCommand => new AsyncCommand(() => BackRequested);

        public Func<Task>? CloseRequested { get; set; }
        public ICommand CloseRequestedCommand => new AsyncCommand(() => CloseRequested);

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
            _logger.LogInformation("{ClassName} is not required to handle deeplinks", nameof(CloseSlimBackToHomeNetworkErrorPage));
            return Task.CompletedTask;
        }
    }
}