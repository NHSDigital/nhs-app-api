using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.DependencyServices.Navigation;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class NhsLoginTermsAndConditionsDeclinedPage : INhsLoginTermsAndConditionsDeclinedView, INhsLoginTermsAndConditionsDeclinedView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<INhsLoginTermsAndConditionsDeclinedView.IEvents> _appNavigation;
        private readonly INavigationService _navigationService;

        public NhsLoginTermsAndConditionsDeclinedPage(ILogger<NhsLoginTermsAndConditionsDeclinedPage> logger, IAccessibilityService accessibilityService, INavigationService navigationService): base(accessibilityService)
        {
            _logger = logger;
            _navigationService = navigationService;
            _appNavigation = new AppNavigation<INhsLoginTermsAndConditionsDeclinedView.IEvents>(this, _navigationService);

            InitializeComponent();
        }

        IAppNavigation<INhsLoginTermsAndConditionsDeclinedView.IEvents> INavigationView<INhsLoginTermsAndConditionsDeclinedView.IEvents>.AppNavigation => _appNavigation;

        public Func<Task>? BackToHomeRequested { get; set; }
        public ICommand BackToHomeCommand => new AsyncCommand(() => BackToHomeRequested);

        public Func<Task>? OneOneOneRequested { get; set; }
        public ICommand OneOneOneCommand => new AsyncCommand(() => OneOneOneRequested);

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

        public Task HandleDeeplink(Uri deeplinkUrl)
        {
            _logger.LogInformation("{className} is not required to handle deeplinks", nameof(NhsLoginTermsAndConditionsDeclinedPage));
            return Task.CompletedTask;
        }
    }
}
