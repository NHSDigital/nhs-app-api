using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyServices.Navigation;
using NHSOnline.App.Navigation;
using NHSOnline.App.Threading;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.PreHome.Views
{
    [DesignTimeVisible(false)]
    public partial class NhsAppPreHomeScreenWebPage : INhsAppPreHomeScreenWebView, INhsAppPreHomeScreenWebView.IEvents, IRootPage
    {
        public event EventHandler<FocusRequestArgs>? AccessibilityFocusChangeRequested;

        private readonly ILogger _logger;
        private readonly AppNavigation<INhsAppPreHomeScreenWebView.IEvents> _appNavigation;

        public NhsAppPreHomeScreenWebPage(ILogger<NhsAppPreHomeScreenWebPage> logger, INavigationService navigationService)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<INhsAppPreHomeScreenWebView.IEvents>(this, navigationService);

            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        IAppNavigation<INhsAppPreHomeScreenWebView.IEvents> INavigationView<INhsAppPreHomeScreenWebView.IEvents>.AppNavigation => _appNavigation;

        Func<Task>? INhsAppPreHomeScreenWebView.IEvents.Appearing { get; set; }
        private AsyncCommand AppearingCommand
            => new AsyncCommand(() => Events.Appearing);

        public Func<Task>? GetNotificationsStatusRequested { get; set; }
        public AsyncCommand GetNotificationsStatusCommand
            => new AsyncCommand(() => GetNotificationsStatusRequested);

        public Func<string, Task>? FetchBiometricStatusRequested { get; set; }
        public AsyncCommand<string> FetchBiometricStatusCommand
            => new AsyncCommand<string>(() => FetchBiometricStatusRequested);

        public Func<string, Task>? UpdateBiometricRegistrationRequested { get; set; }
        public AsyncCommand<string> UpdateBiometricRegistrationCommand
            => new AsyncCommand<string>(() => UpdateBiometricRegistrationRequested);

        public Func<string, Task>? GetPnsTokenRequested { get; set; }
        public AsyncCommand<string> RequestPnsTokenCommand
            => new AsyncCommand<string>(() => GetPnsTokenRequested);

        public Func<Task>? GoToLoggedInHomeRequested { get; set; }
        public AsyncCommand GoToLoggedInHomeCommand
            => new AsyncCommand(() => GoToLoggedInHomeRequested);

        public Func<Task>? LogoutRequested { get; set; }
        public AsyncCommand LogoutCommand
            => new AsyncCommand(() => LogoutRequested);

        public Action<WebNavigatingEventArgs>? Navigating { get; set; }

        public Func<Uri, Task>? NavigationFailed { get; set; }
        private AsyncCommand<Uri> NavigationFailedCommand
            => new AsyncCommand<Uri>(() => NavigationFailed);

        public Func<Task>? OnSessionExpiringRequested { get; set; }
        public AsyncCommand OnSessionExpiringCommand
            => new AsyncCommand(() => OnSessionExpiringRequested);

        public Func<Task>? SessionExpiredRequested { get; set; }
        public AsyncCommand SessionExpiredCommand
            => new AsyncCommand(() => SessionExpiredRequested);

        public Func<Task>? ResetAndShowErrorRequested { get; set; }
        public async Task ResetAndShowError()
        {
            if (Events.ResetAndShowErrorRequested != null)
            {
                await Events.ResetAndShowErrorRequested().PreserveThreadContext();
            }
        }

        public Func<Uri, Task>? ReloadUrlRequested { get; set; }
        public AsyncCommand<Uri> ReloadUrlCommand
            => new AsyncCommand<Uri>(() => ReloadUrlRequested);

        public Func<Uri, Task>? DeeplinkRequested { get; set; }

        public Action<WebViewPageNavigationEventArgs>? PageLoadComplete { get; set; }

        private INhsAppPreHomeScreenWebView.IEvents Events => this;

        protected override void OnAppearing()
        {
            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();

            RemoveEventHandlers();
            AddEventHandlers();

            AppearingCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            _logger.LogInformation("{Method}", nameof(OnDisappearing));
            _appNavigation.SuppressHandlers();

            RemoveEventHandlers();
        }

        private void AddEventHandlers()
        {
            WebView.Navigating += WebViewOnNavigating;
            WebView.Navigated += WebViewOnNavigated;
            WebView.PageLoadComplete += OnPageLoadComplete;
        }

        private void RemoveEventHandlers()
        {
            WebView.Navigating -= WebViewOnNavigating;
            WebView.Navigated -= WebViewOnNavigated;
            WebView.PageLoadComplete -= OnPageLoadComplete;
        }

        private void WebViewOnNavigating(object sender, WebNavigatingEventArgs args)
        {
            _logger.LogInformation("Navigating: {Uri}", args.Url);
            NhsAppResilience.ExecuteImmediately(() => Navigating?.Invoke(args));
            if (args.Cancel)
            {
                ShowWebView();
            }
        }

        private void WebViewOnNavigated(object sender, WebNavigatedEventArgs args)
        {
            _logger.LogInformation("Navigated ({Result}): {Uri}", args.Result, args.Url);

            if (args.Url is null)
            {
                _logger.LogWarning("Web navigation cancelled - url is null - logging out");
                LogoutCommand.Execute(null);
                return;
            }

            switch(args.Result)
            {
                case WebNavigationResult.Success:
                    WebView.Focus();
                    WebView.AccessibilityFocus();
                    break;
                case WebNavigationResult.Cancel:
                    _logger.LogInformation("Web navigation was cancelled");
                    break;
                case WebNavigationResult.Failure:
                case WebNavigationResult.Timeout:
                default:
                    WebView.IsVisible = false;
                    NavigationFailedCommand.Execute(new Uri(args.Url));
                    break;
            }
        }

        public async Task<Uri?> GetCurrentWebViewUrl()
        {
            var currentUrl = await WebView.GetCurrentWebViewUrl().ResumeOnThreadPool();
            return Uri.TryCreate(currentUrl, UriKind.Absolute, out Uri uri) ? uri : null;
        }

        public void GoToUri(Uri uri) => WebView.GoToUri(uri);

        public async Task SendBiometricStatus(BiometricStatus biometricStatus)
            => await WebView.SendBiometricStatus(biometricStatus).ResumeOnThreadPool();

        public async Task SendBiometricCompletion(BiometricCompletion completionDetails)
            => await WebView.SendBiometricCompletion(completionDetails).ResumeOnThreadPool();

        public async Task SendNotificationsStatus(string status)
            => await WebView.SendNotificationsStatus(status).ResumeOnThreadPool();

        public async Task SendNotificationAuthorised(NotificationAuthorisedResponse authorisedResponse)
            => await WebView.SendNotificationAuthorised(authorisedResponse).ResumeOnThreadPool();

        public async Task SendNotificationUnauthorised()
            => await WebView.SendNotificationUnauthorised().ResumeOnThreadPool();

        public async Task SendSessionExtend()
            => await WebView.SendSessionExtend().PreserveThreadContext();

        public async Task Logout()
        {
            await WebView.AuthLogout().PreserveThreadContext();
            LogoutCommand.Execute(null);
        }

        private void WebViewNavigating (object sender, WebNavigatingEventArgs e)
        {
            ShowSpinner();
        }

        private void WebOnEndNavigating (object sender, WebNavigatedEventArgs e)
        {
            ShowWebView();
        }

        private void OnPageLoadComplete(object sender, WebViewPageNavigationEventArgs pageNavigationEventArgs)
        {
            if (PageLoadComplete != null)
            {
                NhsAppResilience.ExecuteOnMainThread(() => PageLoadComplete.Invoke(pageNavigationEventArgs));
            }
        }

        private void ShowWebView()
        {
            Spinner.IsVisible = false;
            WebView.IsVisible = true;
            AccessibilityFocus();
        }

        private void ShowSpinner()
        {
            Spinner.IsVisible = true;
            Spinner.AccessibilityFocus();
            WebView.IsVisible = false;
        }

        protected override bool OnBackButtonPressed() => true;

        public async Task HandleDeeplink(Uri deeplinkUrl)
        {
            if (DeeplinkRequested != null)
            {
                await DeeplinkRequested(deeplinkUrl).PreserveThreadContext();
            }
        }

        private void AccessibilityFocus()
        {
            if (AccessibilityFocusChangeRequested != null)
            {
                var arg = new FocusRequestArgs {Focus = true};
                AccessibilityFocusChangeRequested(this, arg);
            }
        }
    }
}