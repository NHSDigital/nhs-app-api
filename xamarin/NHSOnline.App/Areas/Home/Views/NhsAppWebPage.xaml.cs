using System;
using System.ComponentModel;
using System.Threading.Tasks;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Threading;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.Home.Views
{
    [DesignTimeVisible(false)]
    public partial class NhsAppWebPage : INhsAppWebView, INhsAppWebView.IEvents, IRootPage
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<INhsAppWebView.IEvents> _appNavigation;

        public NhsAppWebPage(ILogger<NhsAppWebPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<INhsAppWebView.IEvents>(this, Navigation);

            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        IAppNavigation<INhsAppWebView.IEvents> INavigationView<INhsAppWebView.IEvents>.AppNavigation => _appNavigation;

        Func<Task>? INhsAppWebView.IEvents.Appearing { get; set; }
        private AsyncCommand AppearingCommand
            => new AsyncCommand(() => ((INhsAppWebView.IEvents)this).Appearing);

        public Func<OpenWebIntegrationRequest, Task>? OpenWebIntegrationRequested { get; set; }
        public AsyncCommand<OpenWebIntegrationRequest> OpenWebIntegrationCommand
            => new AsyncCommand<OpenWebIntegrationRequest>(() => OpenWebIntegrationRequested);

        public Func<StartNhsLoginUpliftRequest, Task>? StartNhsLoginUpliftRequested { get; set; }
        public AsyncCommand<StartNhsLoginUpliftRequest> StartNhsLoginUpliftCommand
            => new AsyncCommand<StartNhsLoginUpliftRequest>(() => StartNhsLoginUpliftRequested);

        public Func<Task>? GetNotificationsStatusRequested { get; set; }
        public AsyncCommand GetNotificationsStatusCommand
            => new AsyncCommand(() => GetNotificationsStatusRequested);

        public Func<string, Task>? GetPnsTokenRequested { get; set; }
        public AsyncCommand<string> RequestPnsTokenCommand
            => new AsyncCommand<string>(() => GetPnsTokenRequested);

        public Func<Task>? FetchBiometricSpecRequested { get; set; }
        public AsyncCommand FetchBiometricSpecCommand
            => new AsyncCommand(() => FetchBiometricSpecRequested);

        public Func<string, Task>? UpdateBiometricRegistrationRequested { get; set; }
        public AsyncCommand<string> UpdateBiometricRegistrationCommand
            => new AsyncCommand<string>(() => UpdateBiometricRegistrationRequested);

        public Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }
        private AsyncCommand<WebNavigatingEventArgs> NavigatingCommand
            => new AsyncCommand<WebNavigatingEventArgs>(() => Navigating);

        public Func<WebNavigatedEventArgs, Task>? Navigated { get; set; }
        private AsyncCommand<WebNavigatedEventArgs> NavigatedCommand
            => new AsyncCommand<WebNavigatedEventArgs>(() => Navigated);

        public Func<Task>? ResetAndShowErrorRequested { get; set; }
        public async Task ResetAndShowError()
        {
            if (ResetAndShowErrorRequested != null)
            {
                await ResetAndShowErrorRequested().ResumeOnThreadPool();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();

            RemoveEventHandlers();
            AddEventHandlers();

            AppearingCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _logger.LogInformation("{Method}", nameof(OnDisappearing));
            _appNavigation.SuppressHandlers();

            RemoveEventHandlers();
        }

        private void AddEventHandlers()
        {
            WebView.Navigating += WebViewOnNavigating;
            WebView.Navigated += WebViewOnNavigated;
        }

        private void RemoveEventHandlers()
        {
            WebView.Navigating -= WebViewOnNavigating;
            WebView.Navigated -= WebViewOnNavigated;
        }

        private void WebViewOnNavigating(object sender, WebNavigatingEventArgs args)
            => NavigatingCommand.Execute(args);

        private void WebViewOnNavigated(object sender, WebNavigatedEventArgs args)
            => NavigatedCommand.Execute(args);

        public void GoToUri(Uri uri) => WebView.GoToUri(uri);

        public async Task NavigateToAdvice() => await WebView.NavigateToAdvice().ResumeOnThreadPool();

        public async Task NavigateToAppointments() => await WebView.NavigateToAppointments().ResumeOnThreadPool();

        public async Task NavigateToPrescriptions() => await WebView.NavigateToPrescriptions().ResumeOnThreadPool();

        public async Task NavigateToYourHealth() => await WebView.NavigateToYourHealth().ResumeOnThreadPool();

        public async Task NavigateToMessages() => await WebView.NavigateToMessages().ResumeOnThreadPool();

        public async Task NavigateToSettings() => await WebView.NavigateToSettings().ResumeOnThreadPool();

        public async Task NavigateToHome() => await WebView.NavigateToHome().ResumeOnThreadPool();

        public async Task NavigateToRedirectedPathWithinApp(string spaPath)
            => await WebView.NavigateToRedirectedPathWithinApp(spaPath).ResumeOnThreadPool();

        public async Task SendNotificationsStatus(string status)
            => await WebView.SendNotificationsStatus(status).ResumeOnThreadPool();

        public async Task SendBiometricSpec(BiometricSpec biometricSpec)
            => await WebView.SendBiometricSpec(biometricSpec).ResumeOnThreadPool();

        public async Task SendBiometricCompletion(BiometricCompletion completionDetails)
            => await WebView.SendBiometricCompletion(completionDetails).ResumeOnThreadPool();

        public async Task SendNotificationAuthorised(NotificationAuthorisedResponse authorisedResponse)
            => await WebView.SendNotificationAuthorised(authorisedResponse).ResumeOnThreadPool();

        public async Task SendNotificationUnauthorised() => await WebView.SendNotificationUnauthorised().ResumeOnThreadPool();

        private void WebViewNavigating(object sender, WebNavigatingEventArgs e)
        {
            Spinner.IsVisible = true;
            WebView.IsVisible = false;
        }

        private void WebOnEndNavigating(object sender, WebNavigatedEventArgs e)
        {
            Spinner.IsVisible = false;
            WebView.IsVisible = true;
        }
    }
}
