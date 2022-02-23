using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Threading;
using Microsoft.Extensions.Logging;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.DependencyServices.Navigation;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.Home.Views
{
    [DesignTimeVisible(false)]
    public partial class NhsAppWebPage : INhsAppWebView, INhsAppWebView.IEvents, IRootPage, ISwipeablePage
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<INhsAppWebView.IEvents> _appNavigation;
        private readonly INavigationService _navigationService;

        public NhsAppWebPage(ILogger<NhsAppWebPage> logger, IAccessibilityService accessibilityService, INavigationService navigationService): base(accessibilityService)
        {
            _logger = logger;
            _navigationService = navigationService;
            _appNavigation = new AppNavigation<INhsAppWebView.IEvents>(this, _navigationService);

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

        public Func<OpenPostWebIntegrationRequest, Task>? OpenPostWebIntegrationRequested { get; set; }
        public AsyncCommand<OpenPostWebIntegrationRequest> OpenPostWebIntegrationCommand
            => new AsyncCommand<OpenPostWebIntegrationRequest>(() => OpenPostWebIntegrationRequested);

        public Func<AddEventToCalendarRequest, Task>? AddEventToCalendarRequested { get; set; }
        public AsyncCommand<AddEventToCalendarRequest> AddEventToCalendarCommand
            => new AsyncCommand<AddEventToCalendarRequest>(() => AddEventToCalendarRequested);

        public Func<DownloadRequest, Task>? StartDownloadRequested { get; set; }
        public AsyncCommand<DownloadRequest> StartDownloadCommand
            => new AsyncCommand<DownloadRequest>(() => StartDownloadRequested);

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

        public Func<Task>? FetchNativeAppVersionRequested { get; set; }
        public AsyncCommand FetchNativeAppVersionCommand
            => new AsyncCommand(() => FetchNativeAppVersionRequested);

        public Func<string, Task>? SetMenuBarItemRequested { get; set; }
        public AsyncCommand<string> SetMenuBarItemCommand
            => new AsyncCommand<string>(() => SetMenuBarItemRequested);

        public Func<Task>? ClearMenuBarItemRequested { get; set; }
        public AsyncCommand ClearMenuBarItemCommand
            => new AsyncCommand(() => ClearMenuBarItemRequested);

        public Func<Task>? DisplayPageLeaveWarningRequested { get; set; }
        public AsyncCommand DisplayPageLeaveWarningCommand
            => new AsyncCommand(() => DisplayPageLeaveWarningRequested);

        public Func<Task>? OnSessionExpiringRequested { get; set; }
        public AsyncCommand OnSessionExpiringCommand
            => new AsyncCommand(() => OnSessionExpiringRequested);

        public Func<string, Task>? UpdateBiometricRegistrationRequested { get; set; }
        public AsyncCommand<string> UpdateBiometricRegistrationCommand
            => new AsyncCommand<string>(() => UpdateBiometricRegistrationRequested);

        public Func<Uri, Task>? OpenBrowserOverlayRequested { get; set; }
        public AsyncCommand<Uri> OpenBrowserOverlayCommand
            => new AsyncCommand<Uri>(() => OpenBrowserOverlayRequested);

        public Func<Task>? OpenSettingsRequested { get; set; }
        public AsyncCommand OpenSettingsCommand
            => new AsyncCommand(() => OpenSettingsRequested);

        public Func<Task>? LogoutRequested { get; set; }
        public AsyncCommand LogoutCommand
            => new AsyncCommand(() => LogoutRequested);

        public Func<Task>? SessionExpiredRequested { get; set; }
        public AsyncCommand SessionExpiredCommand
            => new AsyncCommand(() => SessionExpiredRequested);

        public Func<bool, Task>? BackRequested { get; set; }
        public AsyncCommand<bool> BackRequestedCommand
            => new AsyncCommand<bool>(() => BackRequested);

        public Func<CreateOnDemandGpSessionRequest, Task>? CreateOnDemandGpSessionRequested { get; set; }
        public AsyncCommand<CreateOnDemandGpSessionRequest> CreateOnDemandGpSessionRequestedCommand
            => new AsyncCommand<CreateOnDemandGpSessionRequest>(() => CreateOnDemandGpSessionRequested);

        public Func<Uri, Task>? DeeplinkRequested { get; set; }
        public async Task HandleDeeplink(Uri deeplinkUrl)
        {
            if (DeeplinkRequested != null)
            {
                await DeeplinkRequested(deeplinkUrl).PreserveThreadContext();
            }
        }

        public Action<WebNavigatingEventArgs>? Navigating { get; set; }

        public Func<Uri, Task>? NavigationFailed { get; set; }
        private AsyncCommand<Uri> NavigationFailedCommand
            => new AsyncCommand<Uri>(() => NavigationFailed);

        public Func<Task>? ResetAndShowErrorRequested { get; set; }
        public async Task ResetAndShowError()
        {
            if (ResetAndShowErrorRequested != null)
            {
                await ResetAndShowErrorRequested().ResumeOnThreadPool();
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();

            RemoveEventHandlers();
            AddEventHandlers();

            await ValidateSession().PreserveThreadContext();

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
        {
            _logger.LogInformation("Navigating: {Uri}", args.Url);
            NhsAppResilience.ExecuteImmediately(() => Navigating?.Invoke(args));
        }

        private void WebViewOnNavigated(object sender, WebNavigatedEventArgs args)
        {
            _logger.LogInformation("Navigated ({Result}): {Uri}", args.Result, args.Url);

            if (args.Result != WebNavigationResult.Success && args.Result != WebNavigationResult.Cancel)
            {
                WebView.IsVisible = false;
                NavigationFailedCommand.Execute(new Uri(args.Url));
            }
            else if (args.Result is WebNavigationResult.Cancel)
            {
                _logger.LogInformation("Web navigation was cancelled");
            }

        }

        protected override bool OnBackButtonPressed()
        {
            BackRequestedCommand.Execute(true);
            return true;
        }

        public async Task Logout()
        {
            await WebView.AuthLogout().PreserveThreadContext();
            LogoutCommand.Execute(null);
        }

        public async Task<string> GetCurrentRouteName()
            => await WebView.GetCurrentRouteName().PreserveThreadContext();

        public async Task NavigateBack()
        {
            WebView.Focus();
            WebView.AccessibilityFocus();
            await WebView.NavigateBack().PreserveThreadContext();
        }

        public void GoToUri(Uri uri) => WebView.GoToUri(uri);

        public async Task NavigateToAdvice()
        {
            WebView.Focus();
            WebView.AccessibilityFocus();
            await WebView.NavigateToAdvice().ResumeOnThreadPool();
        }

        public async Task NavigateToAppointments()
        {
            WebView.Focus();
            WebView.AccessibilityFocus();
            await WebView.NavigateToAppointments().ResumeOnThreadPool();
        }

        public async Task NavigateToPrescriptions()
        {
            WebView.Focus();
            WebView.AccessibilityFocus();
            await WebView.NavigateToPrescriptions().ResumeOnThreadPool();
        }

        public async Task NavigateToYourHealth()
        {
            WebView.Focus();
            WebView.AccessibilityFocus();
            await WebView.NavigateToYourHealth().ResumeOnThreadPool();
        }

        public async Task NavigateToMessages()
        {
            WebView.Focus();
            WebView.AccessibilityFocus();
            await WebView.NavigateToMessages().ResumeOnThreadPool();
        }

        public async Task NavigateToHome()
        {
            WebView.Focus();
            WebView.AccessibilityFocus();
            await WebView.NavigateToHome().ResumeOnThreadPool();
        }

        public async Task NavigateToMore()
        {
            WebView.Focus();
            WebView.AccessibilityFocus();
            await WebView.NavigateToMore().ResumeOnThreadPool();
        }

        public async Task NavigateToRedirector(Uri targetUrl)
        {
            WebView.Focus();
            WebView.AccessibilityFocus();
            SelectedNavigationFooterItem = NavigationFooterItem.None;
            await WebView.NavigateToRedirector(targetUrl).ResumeOnThreadPool();
        }

        public Task NavigateToOnDemandGpReturn(Dictionary<string, string> queryParameters)
        {
            WebView.Focus();
            WebView.AccessibilityFocus();
            return WebView.NavigateToOnDemandGpReturn(queryParameters);
        }

        public async Task NavigateToAppPage(string page)
        {
            WebView.Focus();
            WebView.AccessibilityFocus();
            await WebView.NavigateToAppPage(page).ResumeOnThreadPool();
        }

        public async Task UpdateNativeVersion(string version)
            => await WebView.AppVersionUpdateNativeVersion(version).ResumeOnThreadPool();

        public View GetWebViewElement() => WebView;

        public async Task SendNotificationsStatus(string status)
            => await WebView.SendNotificationsStatus(status).ResumeOnThreadPool();

        public async Task SendBiometricStatus(BiometricStatus biometricStatus)
            => await WebView.SendBiometricStatus(biometricStatus).ResumeOnThreadPool();

        public async Task SendBiometricCompletion(BiometricCompletion completionDetails)
            => await WebView.SendBiometricCompletion(completionDetails).ResumeOnThreadPool();

        public async Task SendLeavePage()
            => await WebView.SendPageLeave().PreserveThreadContext();

        public async Task SendStayOnPage()
            => await WebView.SendStayOnPage().PreserveThreadContext();

        public async Task SendSessionExtend()
            => await WebView.SendSessionExtend().PreserveThreadContext();

        public async Task SendNotificationAuthorised(NotificationAuthorisedResponse authorisedResponse)
            => await WebView.SendNotificationAuthorised(authorisedResponse).ResumeOnThreadPool();

        public async Task SendNotificationUnauthorised()
            => await WebView.SendNotificationUnauthorised().ResumeOnThreadPool();

        public async Task ValidateSession()
            => await WebView.ValidateSession().PreserveThreadContext();

        public async Task GetContextualHelpLink()
            => await WebView.GetContextualHelpLink().PreserveThreadContext();

        private void WebOnEndNavigating(object sender, WebNavigatedEventArgs e)
        {
            Spinner.IsVisible = false;
            WebView.IsVisible = true;
        }

        public bool OnSwipeBack()
        {
            BackRequestedCommand.Execute(false);
            return true;
        }
    }
}
