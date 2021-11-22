using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.DependencyServices.Navigation;
using NHSOnline.App.Navigation;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class GettingStartedPage: IGettingStartedView, IGettingStartedView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<IGettingStartedView.IEvents> _appNavigation;
        private readonly INavigationService _navigationService;

        public GettingStartedPage(
            ILogger<GettingStartedPage> logger,
            IAccessibilityService accessibilityService, INavigationService navigationService)
            : base(accessibilityService)
        {
            _logger = logger;
            _navigationService = navigationService;
            _appNavigation = new AppNavigation<IGettingStartedView.IEvents>(this, _navigationService);

            InitializeComponent();
        }

        IAppNavigation<IGettingStartedView.IEvents> INavigationView<IGettingStartedView.IEvents>.AppNavigation => _appNavigation;

        public Func<Task>? LoginRequested { get; set; }
        public ICommand LoginCommand => new AsyncCommand(() => LoginRequested);

        public Func<Task>? WhoCanAccessTheAppRequested { get; set; }
        public ICommand WhoCanAccessTheAppCommand =>
            new AsyncCommand(() => WhoCanAccessTheAppRequested);

        public Func<Task>? BackRequested { get; set; }
        private ICommand BackRequestedCommand => new AsyncCommand(() => BackRequested);

        public Func<Uri, Task>? DeeplinkRequested { get; set; }

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

        public async Task HandleDeeplink(Uri deeplinkUrl)
        {
            if (DeeplinkRequested != null)
            {
                await DeeplinkRequested(deeplinkUrl).PreserveThreadContext();
            }
        }
    }
}
