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
    public partial class BiometricLoginLegacySensorNotValidPage : IBiometricLoginLegacySensorNotValidView,
        IBiometricLoginLegacySensorNotValidView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<IBiometricLoginLegacySensorNotValidView.IEvents> _appNavigation;

        public BiometricLoginLegacySensorNotValidPage(ILogger<BiometricLoginLegacySensorNotValidPage> logger,
            IAccessibilityService accessibilityService) : base(accessibilityService)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<IBiometricLoginLegacySensorNotValidView.IEvents>(this, Navigation);

            InitializeComponent();
        }

        IAppNavigation<IBiometricLoginLegacySensorNotValidView.IEvents>
            INavigationView<IBiometricLoginLegacySensorNotValidView.IEvents>.AppNavigation => _appNavigation;

        Func<Task>? IBiometricLoginLegacySensorNotValidView.IEvents.Appearing { get; set; }
        private ICommand AppearingCommand => new AsyncCommand(() => Events.Appearing);

        public Func<Task>? BackToLoginRequested { get; set; }
        public ICommand BackToLoginCommand => new AsyncCommand(() => BackToLoginRequested);

        private IBiometricLoginLegacySensorNotValidView.IEvents Events => this;

        protected override void OnAppearing()
        {
            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();

            AppearingCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            _logger.LogInformation("{Method}", nameof(OnDisappearing));
            _appNavigation.SuppressHandlers();
        }

        public Task HandleDeeplink(Uri deeplinkUrl)
        {
            _logger.LogInformation("{className} is not required to handle deeplinks",
                nameof(BiometricLoginLegacySensorNotValidPage));
            return Task.CompletedTask;
        }
    }
}