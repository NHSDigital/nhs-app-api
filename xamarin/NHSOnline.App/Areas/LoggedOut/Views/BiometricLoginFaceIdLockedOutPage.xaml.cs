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
    public partial class BiometricLoginFaceIdLockedOutPage : IBiometricLoginFaceIdLockedOutView, IBiometricLoginFaceIdLockedOutView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<IBiometricLoginFaceIdLockedOutView.IEvents> _appNavigation;
        private readonly INavigationService _navigationService;

        public BiometricLoginFaceIdLockedOutPage(ILogger<BiometricLoginFaceIdLockedOutPage> logger, IAccessibilityService accessibilityService, INavigationService navigationService): base(accessibilityService)
        {
            _logger = logger;
            _navigationService = navigationService;
            _appNavigation = new AppNavigation<IBiometricLoginFaceIdLockedOutView.IEvents>(this, _navigationService);

            InitializeComponent();
        }

        IAppNavigation<IBiometricLoginFaceIdLockedOutView.IEvents> INavigationView<IBiometricLoginFaceIdLockedOutView.IEvents>.AppNavigation => _appNavigation;

        Func<Task>? IBiometricLoginFaceIdLockedOutView.IEvents.Appearing { get; set; }
        private ICommand AppearingCommand => new AsyncCommand(() => Events.Appearing);

        public Func<Task>? BackHomeRequested { get; set; }
        public ICommand BackHomeCommand => new AsyncCommand(() => BackHomeRequested);

        private IBiometricLoginFaceIdLockedOutView.IEvents Events => this;

        protected override void OnAppearing()
        {
            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();

            AppearingCommand.Execute(null);

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            _logger.LogInformation("{Method}", nameof(OnDisappearing));
            _appNavigation.SuppressHandlers();
        }

        public Task HandleDeeplink(Uri deeplinkUrl)
        {
            _logger.LogInformation("{className} is not required to handle deeplinks", nameof(BiometricLoginFaceIdLockedOutPage));
            return Task.CompletedTask;
        }
    }
}