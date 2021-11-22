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
    public partial class BiometricLoginFaceIdFailedPage : IBiometricLoginFaceIdFailedView, IBiometricLoginFaceIdFailedView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<IBiometricLoginFaceIdFailedView.IEvents> _appNavigation;
        private readonly INavigationService _navigationService;

        public BiometricLoginFaceIdFailedPage(ILogger<BiometricLoginFaceIdFailedPage> logger, IAccessibilityService accessibilityService, INavigationService navigationService): base(accessibilityService)
        {
            _logger = logger;
            _navigationService = navigationService;
            _appNavigation = new AppNavigation<IBiometricLoginFaceIdFailedView.IEvents>(this, _navigationService);

            InitializeComponent();
        }

        IAppNavigation<IBiometricLoginFaceIdFailedView.IEvents> INavigationView<IBiometricLoginFaceIdFailedView.IEvents>.AppNavigation => _appNavigation;

        public Func<Task>? BackHomeRequested { get; set; }

        public ICommand BackHomeCommand => new AsyncCommand(() => BackHomeRequested);

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
            _logger.LogInformation("{className} is not required to handle deeplinks", nameof(BiometricLoginFaceIdFailedPage));
            return Task.CompletedTask;
        }
    }
}