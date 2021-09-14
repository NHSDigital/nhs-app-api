using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Navigation;
using NHSOnline.App.Threading;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class LoggedOutHomeScreenPage: ILoggedOutHomeScreenView, ILoggedOutHomeScreenView.IEvents, IRootPage
    {
        public static readonly BindableProperty VersionLabelTextProperty
            = BindableProperty.Create(nameof(VersionLabelText), typeof(string), typeof(LoggedOutHomeScreenPage), "");

        private readonly ILogger _logger;
        private readonly AppNavigation<ILoggedOutHomeScreenView.IEvents> _appNavigation;

        public LoggedOutHomeScreenPage(ILogger<LoggedOutHomeScreenPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<ILoggedOutHomeScreenView.IEvents>(this, Navigation);

            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
        }

        IAppNavigation<ILoggedOutHomeScreenView.IEvents> INavigationView<ILoggedOutHomeScreenView.IEvents>.AppNavigation => _appNavigation;

        Func<Task>? ILoggedOutHomeScreenView.IEvents.Appearing { get; set; }
        private ICommand AppearingCommand => new AsyncCommand(() => Events.Appearing);

        Func<Task>? ILoggedOutHomeScreenView.IEvents.Disappearing { get; set; }
        private ICommand DisappearingCommand => new AsyncCommand(() => Events.Disappearing);

        public Func<Task>? LoginRequested { get; set; }
        public ICommand LoginCommand => new AsyncCommand(() => LoginRequested);

        public Func<Task>? NhsUkLoginHelpServicePageRequested { get; set; }
        public ICommand NhsUkLoginHelpServiceCommand => new AsyncCommand(() => NhsUkLoginHelpServicePageRequested);

        public Func<Task>? BackRequested { get; set; }
        private ICommand BackRequestedCommand => new AsyncCommand(() => BackRequested);

        public Func<Task>? ResetAndShowErrorRequested { get; set; }
        public Func<Uri, Task>? DeeplinkRequested { get; set; }

        public string VersionLabelText
        {
            get => (string)GetValue(VersionLabelTextProperty);
            set => SetValue(VersionLabelTextProperty, value);
        }

        public async Task ResetAndShowError()
        {
            if (ResetAndShowErrorRequested != null)
            {
                await ResetAndShowErrorRequested().PreserveThreadContext();
            }
        }

        private ILoggedOutHomeScreenView.IEvents Events => this;

        protected override void OnAppearing()
        {
            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();

            AppearingCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            DisappearingCommand.Execute(null);

            _logger.LogInformation("{Method}", nameof(OnDisappearing));
            _appNavigation.SuppressHandlers();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            ResponsiveStates.SetVisualStateBreakpoints(ContentView);
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

        public void ResetScreenState()
        {
            VisualStateManager.GoToState(
                SessionExpiredBanner,
                LoggedOutHomeScreenStates.Default.ToString());
        }

        public void SetScreenState(LoggedOutHomeScreenStates loggedOutHomeScreenState)
        {
            VisualStateManager.GoToState(
                SessionExpiredBanner,
                loggedOutHomeScreenState.ToString());
        }
    }
}
