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
    public partial class BiometricLoginCouldNotLoginPage : IBiometricLoginCouldNotLoginView, IBiometricLoginCouldNotLoginView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<IBiometricLoginCouldNotLoginView.IEvents> _appNavigation;

        public BiometricLoginCouldNotLoginPage(ILogger<BiometricLoginCouldNotLoginPage> logger, IAccessibilityService accessibilityService): base(accessibilityService)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<IBiometricLoginCouldNotLoginView.IEvents>(this, Navigation);

            InitializeComponent();
        }

        IAppNavigation<IBiometricLoginCouldNotLoginView.IEvents> INavigationView<IBiometricLoginCouldNotLoginView.IEvents>.AppNavigation => _appNavigation;

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
            _logger.LogInformation("{className} is not required to handle deeplinks", nameof(BiometricLoginCouldNotLoginPage));
            return Task.CompletedTask;
        }
    }
}