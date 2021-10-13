using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class BiometricLoginFingerprintFailedPage : IBiometricLoginFingerprintFaceIrisFailedView, IBiometricLoginFingerprintFaceIrisFailedView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<IBiometricLoginFingerprintFaceIrisFailedView.IEvents> _appNavigation;

        public BiometricLoginFingerprintFailedPage(ILogger<BiometricLoginFingerprintFailedPage> logger, IAccessibilityService accessibilityService): base(accessibilityService)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<IBiometricLoginFingerprintFaceIrisFailedView.IEvents>(this, Navigation);

            InitializeComponent();
        }

        IAppNavigation<IBiometricLoginFingerprintFaceIrisFailedView.IEvents> INavigationView<IBiometricLoginFingerprintFaceIrisFailedView.IEvents>.AppNavigation => _appNavigation;

        public Func<Task>? BackHomeRequested { get; set; }
        public ICommand BackHomeCommand => new AsyncCommand(() => BackHomeRequested);

        protected override void OnAppearing()
        {
            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();

            Heading.AccessibilityFocus();
        }

        protected override void OnDisappearing()
        {
            _logger.LogInformation("{Method}", nameof(OnDisappearing));
            _appNavigation.SuppressHandlers();
        }

        public Task HandleDeeplink(Uri deeplinkUrl)
        {
            _logger.LogInformation("{className} is not required to handle deeplinks", nameof(BiometricLoginFingerprintFailedPage));
            return Task.CompletedTask;
        }
    }
}