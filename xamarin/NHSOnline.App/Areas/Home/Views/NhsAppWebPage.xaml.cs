using System;
using System.ComponentModel;
using System.Threading.Tasks;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Threading;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Navigation;
using NHSOnline.App.Navigation.Pages;
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

        public Func<string, Task>? FetchBiometricStatusRequested { get; set; }
        public AsyncCommand<string> FetchBiometricStatusCommand
            => new AsyncCommand<string>(() => FetchBiometricStatusRequested);

        public Func<string, Task>? SetMenuBarItemRequested { get; set; }
        public AsyncCommand<string> SetMenuBarItemCommand
            => new AsyncCommand<string>(() => SetMenuBarItemRequested);

        public Func<Task>? ClearMenuBarItemRequested { get; set; }
        public AsyncCommand ClearMenuBarItemCommand
            => new AsyncCommand(() => ClearMenuBarItemRequested);

        public Func<string, Task>? UpdateBiometricRegistrationRequested { get; set; }
        public AsyncCommand<string> UpdateBiometricRegistrationCommand
            => new AsyncCommand<string>(() => UpdateBiometricRegistrationRequested);

        public Func<Task>? OpenSettingsRequested { get; set; }
        public AsyncCommand OpenSettingsCommand
            => new AsyncCommand(() => OpenSettingsRequested);

        public Func<Task>? LogoutRequested { get; set; }
        public AsyncCommand LogoutCommand
            => new AsyncCommand(() => LogoutRequested);

        public Func<Uri, Task>? DeeplinkRequested { get; set; }
        public async Task HandleDeeplink(Uri deeplinkUrl)
        {
            if (DeeplinkRequested != null)
            {
                await DeeplinkRequested(deeplinkUrl).PreserveThreadContext();
            }
        }

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
        {
            WebView.Focus();
            WebView.AccessibilityFocus();
            NavigatedCommand.Execute(args);
        }

        public void GoToUri(Uri uri) => WebView.GoToUri(uri);

        public async Task NavigateToAdvice()
        {
            WebView.Focus();
            WebView.AccessibilityFocus();
            await WebView.NavigateToAdvice().ResumeOnThreadPool();
            Page.HighlightedNavigationFooterItem = NavigationFooterItem.Advice;
        }

        public async Task NavigateToAppointments()
        {
            WebView.Focus();
            WebView.AccessibilityFocus();
            await WebView.NavigateToAppointments().ResumeOnThreadPool();
            Page.HighlightedNavigationFooterItem = NavigationFooterItem.Appointments;
        }

        public async Task NavigateToPrescriptions()
        {
            WebView.Focus();
            WebView.AccessibilityFocus();
            await WebView.NavigateToPrescriptions().ResumeOnThreadPool();
            Page.HighlightedNavigationFooterItem = NavigationFooterItem.Prescriptions;
        }

        public async Task NavigateToYourHealth()
        {
            WebView.Focus();
            WebView.AccessibilityFocus();
            await WebView.NavigateToYourHealth().ResumeOnThreadPool();
            Page.HighlightedNavigationFooterItem = NavigationFooterItem.YourHealth;
        }

        public async Task NavigateToMessages()
        {
            WebView.Focus();
            WebView.AccessibilityFocus();
            await WebView.NavigateToMessages().ResumeOnThreadPool();
            Page.HighlightedNavigationFooterItem = NavigationFooterItem.Messages;
        }

        public async Task NavigateToHome()
        {
            WebView.Focus();
            WebView.AccessibilityFocus();
            await WebView.NavigateToHome().ResumeOnThreadPool();
            Page.HighlightedNavigationFooterItem = NavigationFooterItem.None;
        }

        public async Task NavigateToMore()
        {
            WebView.Focus();
            WebView.AccessibilityFocus();
            await WebView.NavigateToMore().ResumeOnThreadPool();
            Page.HighlightedNavigationFooterItem = NavigationFooterItem.None;
        }

        public async Task NavigateToRedirector(Uri targetUrl)
        {
            WebView.Focus();
            WebView.AccessibilityFocus();
            await WebView.NavigateToRedirector(targetUrl).ResumeOnThreadPool();
        }

        public void HighlightNavigationFooterItem(NavigationFooterItem navigationFooterItem) => Page.HighlightedNavigationFooterItem = navigationFooterItem;

        public void ClearHighlightedNavigationFooterItem() => Page.HighlightedNavigationFooterItem = NavigationFooterItem.None;

        public async Task NavigateToRedirectedPathWithinApp(string spaPath)
        {
            WebView.Focus();
            WebView.AccessibilityFocus();
            await WebView.NavigateToRedirectedPathWithinApp(spaPath).ResumeOnThreadPool();
        }

        public async Task SendNotificationsStatus(string status)
            => await WebView.SendNotificationsStatus(status).ResumeOnThreadPool();

        public async Task SendBiometricStatus(BiometricStatus biometricStatus)
            => await WebView.SendBiometricStatus(biometricStatus).ResumeOnThreadPool();

        public async Task SendBiometricCompletion(BiometricCompletion completionDetails)
            => await WebView.SendBiometricCompletion(completionDetails).ResumeOnThreadPool();

        public async Task SendNotificationAuthorised(NotificationAuthorisedResponse authorisedResponse)
            => await WebView.SendNotificationAuthorised(authorisedResponse).ResumeOnThreadPool();

        public async Task SendNotificationUnauthorised() => await WebView.SendNotificationUnauthorised().ResumeOnThreadPool();

        private void WebOnEndNavigating(object sender, WebNavigatedEventArgs e)
        {
            Spinner.IsVisible = false;
            WebView.IsVisible = true;
        }
    }
}
