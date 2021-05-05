using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class BiometricLoginTouchIdLockedOutPage : IBiometricLoginTouchIdLockedOutView, IBiometricLoginTouchIdLockedOutView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<IBiometricLoginTouchIdLockedOutView.IEvents> _appNavigation;

        public BiometricLoginTouchIdLockedOutPage(ILogger<BiometricLoginTouchIdLockedOutPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<IBiometricLoginTouchIdLockedOutView.IEvents>(this, Navigation);

            InitializeComponent();
        }

        IAppNavigation<IBiometricLoginTouchIdLockedOutView.IEvents> INavigationView<IBiometricLoginTouchIdLockedOutView.IEvents>.AppNavigation => _appNavigation;

        Func<Task>? IBiometricLoginTouchIdLockedOutView.IEvents.Appearing { get; set; }
        private ICommand AppearingCommand => new AsyncCommand(() => Events.Appearing);

        public Func<Task>? BackHomeRequested { get; set; }
        public ICommand BackHomeCommand => new AsyncCommand(() => BackHomeRequested);

        private IBiometricLoginTouchIdLockedOutView.IEvents Events => this;

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
            _logger.LogInformation("{className} is not required to handle deeplinks", nameof(BiometricLoginFingerprintLockedOutPage));
            return Task.CompletedTask;
        }
    }
}