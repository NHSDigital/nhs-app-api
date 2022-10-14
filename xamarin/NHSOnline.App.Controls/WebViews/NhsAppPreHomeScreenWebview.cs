using System;
using System.Threading.Tasks;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Threading;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.WebViews
{
    public sealed class NhsAppPreHomeScreenWebview: WebViewBase, IAccessibleControl, INavigationFlowAwareWebView
    {
        public event EventHandler<FocusRequestArgs> AccessibilityFocusChangeRequested = null!;

        public void AccessibilityFocus()
        {
            if (AccessibilityFocusChangeRequested != null)
            {
                var arg = new FocusRequestArgs {Focus = true};
                AccessibilityFocusChangeRequested(this, arg);
            }
        }

        public event EventHandler<WebViewPageNavigationEventArgs>? PageLoadComplete;

        public static readonly BindableProperty GetNotificationsStatusCommandProperty =
            BindableProperty.Create(nameof(GetNotificationsStatusCommand), typeof(AsyncCommand), typeof(NhsAppPreHomeScreenWebview));

        public static readonly BindableProperty FetchBiometricStatusCommandProperty =
            BindableProperty.Create(nameof(FetchBiometricStatusCommand), typeof(AsyncCommand<string>), typeof(NhsAppWebView));

        public static readonly BindableProperty UpdateBiometricRegistrationCommandProperty =
            BindableProperty.Create(nameof(UpdateBiometricRegistrationCommand), typeof(AsyncCommand<string>), typeof(NhsAppWebView));

        public static readonly BindableProperty RequestPnsTokenCommandProperty =
            BindableProperty.Create(nameof(RequestPnsTokenCommand), typeof(AsyncCommand<string>), typeof(NhsAppPreHomeScreenWebview));

        public static readonly BindableProperty GoToLoggedInHomeScreenCommandProperty =
            BindableProperty.Create(nameof(GoToLoggedInHomeScreenCommand), typeof(AsyncCommand), typeof(NhsAppPreHomeScreenWebview));

        public static readonly BindableProperty LogoutCommandProperty =
            BindableProperty.Create(nameof(LogoutCommand), typeof(AsyncCommand), typeof(NhsAppPreHomeScreenWebview));

        public static readonly BindableProperty RequestNotificationsRegistrationCommandProperty =
            BindableProperty.Create(nameof(RequestNotificationsRegistrationCommand), typeof(AsyncCommand<string>), typeof(NhsAppPreHomeScreenWebview));

        public static readonly BindableProperty SetNotificationsRegistrationCommandProperty =
            BindableProperty.Create(nameof(SetNotificationsRegistrationCommand), typeof(AsyncCommand<SetNotificationsRegistrationRequest>), typeof(NhsAppPreHomeScreenWebview));

        public static readonly BindableProperty OnSessionExpiringCommandProperty =
            BindableProperty.Create(nameof(OnSessionExpiringCommand), typeof(AsyncCommand), typeof(NhsAppWebView));

        public static readonly BindableProperty SessionExpiredCommandProperty =
            BindableProperty.Create(nameof(SessionExpiredCommand), typeof(AsyncCommand), typeof(NhsAppWebView));

        public void GetNotificationsStatus() => GetNotificationsStatusCommand.Execute(null);

        public AsyncCommand GetNotificationsStatusCommand
        {
            get => (AsyncCommand) GetValue(GetNotificationsStatusCommandProperty);
            set => SetValue(GetNotificationsStatusCommandProperty, value);
        }

        public void FetchBiometricStatus(string accessToken) => FetchBiometricStatusCommand.Execute(accessToken);

        public AsyncCommand<string> FetchBiometricStatusCommand
        {
            get => (AsyncCommand<string>)GetValue(FetchBiometricStatusCommandProperty);
            set => SetValue(FetchBiometricStatusCommandProperty, value);
        }

        public void UpdateBiometricRegistration(string argument) => UpdateBiometricRegistrationCommand.Execute(argument);

        public AsyncCommand<string> UpdateBiometricRegistrationCommand
        {
            get => (AsyncCommand<string>)GetValue(UpdateBiometricRegistrationCommandProperty);
            set => SetValue(UpdateBiometricRegistrationCommandProperty, value);
        }

        public async Task SendBiometricStatus(BiometricStatus biometricStatus)
        {
            const string callbackName = "window.nativeAppCallbacks.biometricStatus";
            var jsonArgument = ConvertToJsonString(biometricStatus);
            await EvaluateJavaScriptAsync($"{callbackName}({jsonArgument})").ResumeOnThreadPool();
        }

        public async Task SendBiometricCompletion(BiometricCompletion biometricCompletion)
        {
            const string callbackName = "window.nativeAppCallbacks.loginSettingsPreHomeBiometricCompletion";
            var jsonArgument = ConvertToJsonString(biometricCompletion);
            await EvaluateJavaScriptAsync($"{callbackName}({jsonArgument})").ResumeOnThreadPool();
        }

        public async Task SendNotificationsStatus(string status)
        {
            const string callbackName = "window.nativeAppCallbacks.notificationsSettingsStatus";
            var argumentJson = ConvertToJsonString(status);
            await EvaluateJavaScriptAsync($"{callbackName}({argumentJson})").ResumeOnThreadPool();
        }

        public void RequestPnsToken(string argument) => RequestPnsTokenCommand.Execute(argument);

        public AsyncCommand<string> RequestPnsTokenCommand
        {
            get => (AsyncCommand<string>) GetValue(RequestPnsTokenCommandProperty);
            set => SetValue(RequestPnsTokenCommandProperty, value);
        }

        public async Task SendNotificationAuthorised(NotificationAuthorisedResponse authorisedResponse)
        {
            const string callbackName = "window.nativeAppCallbacks.notificationsAuthorised";
            var argumentJson = ConvertToJsonString(authorisedResponse);
            await EvaluateJavaScriptAsync($"{callbackName}({argumentJson})").ResumeOnThreadPool();
        }

        public async Task SendNotificationUnauthorised()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.notificationsUnauthorised()").ResumeOnThreadPool();

        public async Task SendNotificationsRegistration(NotificationsRegistration response)
        {
            const string callbackName = "window.nativeAppCallbacks.setNotificationsRegistration";
            var argumentJson = ConvertToJsonString(response);
            await EvaluateJavaScriptAsync($"{callbackName}({argumentJson})").ResumeOnThreadPool();
        }

        public async Task AuthLogout()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.authLogout()").ResumeOnThreadPool();

        public async Task SendSessionExtend()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.sessionExtend()").ResumeOnThreadPool();

        public async Task<string> GetCurrentWebViewUrl()
            => await EvaluateJavaScriptAsync("window.document.URL").ResumeOnThreadPool();

        public void GoToLoggedInHomeScreen() => GoToLoggedInHomeScreenCommand.Execute(null);

        public AsyncCommand GoToLoggedInHomeScreenCommand
        {
            get => (AsyncCommand) GetValue(GoToLoggedInHomeScreenCommandProperty);
            set => SetValue(GoToLoggedInHomeScreenCommandProperty, value);
        }

        public void Logout() => LogoutCommand.Execute(null);

        public AsyncCommand LogoutCommand
        {
            get => (AsyncCommand) GetValue(LogoutCommandProperty);
            set => SetValue(LogoutCommandProperty, value);
        }

        public void RequestNotificationsRegistration(string nhsLoginId)
            => RequestNotificationsRegistrationCommand.Execute(nhsLoginId);

        public AsyncCommand<string> RequestNotificationsRegistrationCommand
        {
            get => (AsyncCommand<string>) GetValue(RequestNotificationsRegistrationCommandProperty);
            set => SetValue(RequestNotificationsRegistrationCommandProperty, value);
        }

        public void SetNotificationsRegistration(string json)
        {
            var request = ConvertFromJsonString<SetNotificationsRegistrationRequest>(json);
            SetNotificationsRegistrationCommand.Execute(request);
        }

        public AsyncCommand<SetNotificationsRegistrationRequest> SetNotificationsRegistrationCommand
        {
            get => (AsyncCommand<SetNotificationsRegistrationRequest>) GetValue(SetNotificationsRegistrationCommandProperty);
            set => SetValue(SetNotificationsRegistrationCommandProperty, value);
        }

        public void OnSessionExpiring() => OnSessionExpiringCommand.Execute(null);

        public AsyncCommand OnSessionExpiringCommand
        {
            get => (AsyncCommand) GetValue(OnSessionExpiringCommandProperty);
            set => SetValue(OnSessionExpiringCommandProperty, value);
        }

        public void SessionExpired() => SessionExpiredCommand.Execute(null);

        public AsyncCommand SessionExpiredCommand
        {
            get => (AsyncCommand)GetValue(SessionExpiredCommandProperty);
            set => SetValue(SessionExpiredCommandProperty, value);
        }

        void INavigationFlowAwareWebView.OnPageLoadComplete(WebViewPageNavigationEventArgs pageNavigationEventArgs)
        {
            PageLoadComplete?.Invoke(this, pageNavigationEventArgs);
        }
    }
}
