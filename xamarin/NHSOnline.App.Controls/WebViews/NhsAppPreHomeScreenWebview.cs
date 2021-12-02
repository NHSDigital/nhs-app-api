using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Threading;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.WebViews
{
    public sealed class NhsAppPreHomeScreenWebview: WebView, IAccessibleControl, INavigationFlowAwareWebView
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

        public static readonly BindableProperty RequestPnsTokenCommandProperty =
            BindableProperty.Create(nameof(RequestPnsTokenCommand), typeof(AsyncCommand<string>), typeof(NhsAppPreHomeScreenWebview));

        public static readonly BindableProperty GoToLoggedInHomeScreenCommandProperty =
            BindableProperty.Create(nameof(GoToLoggedInHomeScreenCommand), typeof(AsyncCommand), typeof(NhsAppPreHomeScreenWebview));

        public static readonly BindableProperty LogoutCommandProperty =
            BindableProperty.Create(nameof(LogoutCommand), typeof(AsyncCommand), typeof(NhsAppPreHomeScreenWebview));

        public static readonly BindableProperty OnSessionExpiringCommandProperty =
            BindableProperty.Create(nameof(OnSessionExpiringCommand), typeof(AsyncCommand), typeof(NhsAppWebView));

        public static readonly BindableProperty SessionExpiredCommandProperty =
            BindableProperty.Create(nameof(SessionExpiredCommand), typeof(AsyncCommand), typeof(NhsAppWebView));

        private static JsonSerializerSettings Settings { get; } = CreateJsonSerializerSettings();

        public void GetNotificationsStatus() => GetNotificationsStatusCommand.Execute(null);

        public AsyncCommand GetNotificationsStatusCommand
        {
            get => (AsyncCommand) GetValue(GetNotificationsStatusCommandProperty);
            set => SetValue(GetNotificationsStatusCommandProperty, value);
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

        private static string ConvertToJsonString<T>(T value) => JsonConvert.SerializeObject(value, Settings);

        private static JsonSerializerSettings CreateJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
            settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
            return settings;
        }

        void INavigationFlowAwareWebView.OnPageLoadComplete(WebViewPageNavigationEventArgs pageNavigationEventArgs)
        {
            PageLoadComplete?.Invoke(this, pageNavigationEventArgs);
        }
    }
}
