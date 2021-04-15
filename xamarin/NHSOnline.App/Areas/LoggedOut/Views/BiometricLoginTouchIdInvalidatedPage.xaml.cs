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
    public partial class BiometricLoginTouchIdInvalidatedPage : IBiometricLoginTouchIdInvalidatedView, IBiometricLoginTouchIdInvalidatedView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<IBiometricLoginTouchIdInvalidatedView.IEvents> _appNavigation;

        public BiometricLoginTouchIdInvalidatedPage(ILogger<BiometricLoginTouchIdInvalidatedPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<IBiometricLoginTouchIdInvalidatedView.IEvents>(this, Navigation);

            InitializeComponent();
        }

        IAppNavigation<IBiometricLoginTouchIdInvalidatedView.IEvents> INavigationView<IBiometricLoginTouchIdInvalidatedView.IEvents>.AppNavigation => _appNavigation;

        Func<Task>? IBiometricLoginTouchIdInvalidatedView.IEvents.Appearing { get; set; }
        private ICommand AppearingCommand => new AsyncCommand(() => Events.Appearing);

        public Func<Task>? BackHomeRequested { get; set; }
        public ICommand BackHomeCommand => new AsyncCommand(() => BackHomeRequested);

        private IBiometricLoginTouchIdInvalidatedView.IEvents Events => this;

        protected override void OnAppearing()
        {
            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();

            AppearingCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            _logger.LogInformation(nameof(OnDisappearing));

            _logger.LogInformation("{Method}", nameof(OnDisappearing));
            _appNavigation.SuppressHandlers();
        }
    }
}