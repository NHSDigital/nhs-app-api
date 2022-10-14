using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.WebViews
{
    public sealed class NhsAppWebView: WebViewBase, IAccessibleControl
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(NhsAppWebView));

        public event EventHandler<FocusRequestArgs> AccessibilityFocusChangeRequested = null!;

        public void AccessibilityFocus()
        {
            if (AccessibilityFocusChangeRequested != null)
            {
                var arg = new FocusRequestArgs {Focus = true};
                AccessibilityFocusChangeRequested(this, arg);
            }
        }

        public static readonly BindableProperty OpenWebIntegrationCommandProperty =
            BindableProperty.Create(nameof(OpenWebIntegrationCommand), typeof(AsyncCommand<OpenWebIntegrationRequest>), typeof(NhsAppWebView));

        public static readonly BindableProperty OpenPostWebIntegrationCommandProperty =
            BindableProperty.Create(nameof(OpenPostWebIntegrationCommand), typeof(AsyncCommand<OpenPostWebIntegrationRequest>), typeof(NhsAppWebView));

        public static readonly BindableProperty AddEventToCalendarCommandProperty =
            BindableProperty.Create(nameof(AddEventToCalendarCommand), typeof(AsyncCommand<AddEventToCalendarRequest>), typeof(NhsAppWebView));

        public static readonly BindableProperty StartNhsLoginUpliftCommandProperty =
            BindableProperty.Create(nameof(StartNhsLoginUpliftCommand), typeof(AsyncCommand<StartNhsLoginUpliftRequest>), typeof(NhsAppWebView));

        public static readonly BindableProperty GetNotificationsStatusCommandProperty =
            BindableProperty.Create(nameof(GetNotificationsStatusCommand), typeof(AsyncCommand), typeof(NhsAppWebView));

        public static readonly BindableProperty RequestPnsTokenCommandProperty =
            BindableProperty.Create(nameof(RequestPnsTokenCommand), typeof(AsyncCommand<string>), typeof(NhsAppWebView));

        public static readonly BindableProperty FetchBiometricStatusCommandProperty =
            BindableProperty.Create(nameof(FetchBiometricStatusCommand), typeof(AsyncCommand<string>), typeof(NhsAppWebView));

        public static readonly BindableProperty FetchNativeAppVersionCommandProperty =
            BindableProperty.Create(nameof(FetchNativeAppVersionCommand), typeof(AsyncCommand), typeof(NhsAppWebView));

        public static readonly BindableProperty SetMenuBarItemCommandProperty =
            BindableProperty.Create(nameof(SetMenuBarItemCommand), typeof(AsyncCommand<string>), typeof(NhsAppWebView));

        public static readonly BindableProperty ClearMenuBarItemCommandProperty =
            BindableProperty.Create(nameof(ClearMenuBarItemCommand), typeof(AsyncCommand), typeof(NhsAppWebView));

        public static readonly BindableProperty UpdateBiometricRegistrationCommandProperty =
            BindableProperty.Create(nameof(UpdateBiometricRegistrationCommand), typeof(AsyncCommand<string>), typeof(NhsAppWebView));

        public static readonly BindableProperty OpenBrowserOverlayCommandProperty =
            BindableProperty.Create(nameof(OpenBrowserOverlayCommand), typeof(AsyncCommand<Uri>), typeof(NhsAppWebView));

        public static readonly BindableProperty OpenSettingsCommandProperty =
            BindableProperty.Create(nameof(OpenSettingsCommand), typeof(AsyncCommand), typeof(NhsAppWebView));

        public static readonly BindableProperty StartDownloadCommandProperty =
            BindableProperty.Create(nameof(StartDownloadCommand), typeof(AsyncCommand<DownloadRequest>), typeof(NhsAppWebView));

        public static readonly BindableProperty DisplayPageLeaveWarningCommandProperty =
            BindableProperty.Create(nameof(DisplayPageLeaveWarningCommand), typeof(AsyncCommand), typeof(NhsAppWebView));

        public static readonly BindableProperty DisplayKeywordReplyPageLeaveWarningCommandProperty =
            BindableProperty.Create(nameof(DisplayKeywordReplyPageLeaveWarningCommand), typeof(AsyncCommand), typeof(NhsAppWebView));

        public static readonly BindableProperty OnSessionExpiringCommandProperty =
            BindableProperty.Create(nameof(OnSessionExpiringCommandProperty), typeof(AsyncCommand), typeof(NhsAppWebView));

        public static readonly BindableProperty LogoutCommandProperty =
            BindableProperty.Create(nameof(LogoutCommand), typeof(AsyncCommand), typeof(NhsAppWebView));

        public static readonly BindableProperty SessionExpiredCommandProperty =
            BindableProperty.Create(nameof(SessionExpiredCommand), typeof(AsyncCommand), typeof(NhsAppWebView));

        public static readonly BindableProperty CreateOnDemandGpSessionCommandProperty =
            BindableProperty.Create(nameof(CreateOnDemandGpSessionCommand), typeof(AsyncCommand<CreateOnDemandGpSessionRequest>), typeof(NhsAppWebView));

        public static readonly BindableProperty SetBadgeCountCommandProperty =
            BindableProperty.Create(nameof(SetBadgeCountCommand), typeof(AsyncCommand<string>), typeof(NhsAppWebView));

        public static readonly BindableProperty RequestNotificationsRegistrationCommandProperty =
            BindableProperty.Create(nameof(RequestNotificationsRegistrationCommand), typeof(AsyncCommand<string>), typeof(NhsAppWebView));

        public static readonly BindableProperty SetNotificationsRegistrationCommandProperty =
            BindableProperty.Create(nameof(SetNotificationsRegistrationCommand), typeof(AsyncCommand<SetNotificationsRegistrationRequest>), typeof(NhsAppWebView));

        public void OpenWebIntegration(string json)
        {
            var request = ConvertFromJsonString<OpenWebIntegrationRequest>(json);
            OpenWebIntegrationCommand.Execute(request);
        }

        public AsyncCommand<OpenWebIntegrationRequest> OpenWebIntegrationCommand
        {
            get => (AsyncCommand<OpenWebIntegrationRequest>) GetValue(OpenWebIntegrationCommandProperty);
            set => SetValue(OpenWebIntegrationCommandProperty, value);
        }

        public void OpenPostWebIntegration(string json)
        {
            var request = ConvertFromJsonString<OpenPostWebIntegrationRequest>(json);
            OpenPostWebIntegrationCommand.Execute(request);
        }

        public AsyncCommand<OpenPostWebIntegrationRequest> OpenPostWebIntegrationCommand
        {
            get => (AsyncCommand<OpenPostWebIntegrationRequest>) GetValue(OpenPostWebIntegrationCommandProperty);
            set => SetValue(OpenPostWebIntegrationCommandProperty, value);
        }


        public void AddEventToCalendar(string json)
        {
            try
            {
                var request = ConvertFromJsonString<AddEventToCalendarRequest>(json);
                AddEventToCalendarCommand.Execute(request);
            }
            catch (ArgumentException e)
            {
                Logger.LogError(e, "Failed to deserialize the calendar request, not showing any dialogs");
            }
        }

        public AsyncCommand<AddEventToCalendarRequest> AddEventToCalendarCommand
        {
            get => (AsyncCommand<AddEventToCalendarRequest>) GetValue(AddEventToCalendarCommandProperty);
            set => SetValue(AddEventToCalendarCommandProperty, value);
        }

        public void StartDownload(string json)
        {
            try
            {
                var request = ConvertFromJsonString<DownloadRequest>(json);
                StartDownloadCommand.Execute(request);
            }
            catch (ArgumentException e)
            {
                Logger.LogError(e, "Failed to deserialize the download request, not showing any dialogs");
            }
        }

        public AsyncCommand<DownloadRequest> StartDownloadCommand
        {
            get => (AsyncCommand<DownloadRequest>) GetValue(StartDownloadCommandProperty);
            set => SetValue(StartDownloadCommandProperty, value);
        }

        public void StartNhsLoginUplift(string json)
        {
            var request = ConvertFromJsonString<StartNhsLoginUpliftRequest>(json);
            StartNhsLoginUpliftCommand.Execute(request);
        }

        public AsyncCommand<StartNhsLoginUpliftRequest> StartNhsLoginUpliftCommand
        {
            get => (AsyncCommand<StartNhsLoginUpliftRequest>)GetValue(StartNhsLoginUpliftCommandProperty);
            set => SetValue(StartNhsLoginUpliftCommandProperty, value);
        }

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
            await EvaluateJavaScriptWithLoggingAsync($"{callbackName}({argumentJson})").ResumeOnThreadPool();
        }

        public async Task SendNotificationAuthorised(NotificationAuthorisedResponse authorisedResponse)
        {
            const string callbackName = "window.nativeAppCallbacks.notificationsAuthorised";
            var argumentJson = ConvertToJsonString(authorisedResponse);
            await EvaluateJavaScriptWithLoggingAsync($"{callbackName}({argumentJson})").ResumeOnThreadPool();
        }
        public async Task SendNotificationUnauthorised()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.notificationsUnauthorised()").ResumeOnThreadPool();

        public async Task SendNotificationsRegistration(NotificationsRegistration response)
        {
            const string callbackName = "window.nativeAppCallbacks.setNotificationsRegistration";
            var argumentJson = ConvertToJsonString(response);
            await EvaluateJavaScriptAsync($"{callbackName}({argumentJson})").ResumeOnThreadPool();
        }

        public void RequestPnsToken(string argument) => RequestPnsTokenCommand.Execute(argument);

        public AsyncCommand<string> RequestPnsTokenCommand
        {
            get => (AsyncCommand<string>) GetValue(RequestPnsTokenCommandProperty);
            set => SetValue(RequestPnsTokenCommandProperty, value);
        }

        public void FetchBiometricStatus(string accessToken) => FetchBiometricStatusCommand.Execute(accessToken);

        public AsyncCommand<string> FetchBiometricStatusCommand
        {
            get => (AsyncCommand<string>)GetValue(FetchBiometricStatusCommandProperty);
            set => SetValue(FetchBiometricStatusCommandProperty, value);
        }

        public void FetchNativeAppVersion() => FetchNativeAppVersionCommand.Execute(null);

        public AsyncCommand FetchNativeAppVersionCommand
        {
            get => (AsyncCommand) GetValue(FetchNativeAppVersionCommandProperty);
            set => SetValue(FetchNativeAppVersionCommandProperty, value);
        }

        public void SetMenuBarItem(string index) => SetMenuBarItemCommand.Execute(index);

        public AsyncCommand<string> SetMenuBarItemCommand
        {
            get => (AsyncCommand<string>) GetValue(SetMenuBarItemCommandProperty);
            set => SetValue(SetMenuBarItemCommandProperty, value);
        }

        public void ClearMenuBarItem() => ClearMenuBarItemCommand.Execute(null);

        public AsyncCommand ClearMenuBarItemCommand
        {
            get => (AsyncCommand) GetValue(ClearMenuBarItemCommandProperty);
            set => SetValue(ClearMenuBarItemCommandProperty, value);
        }

        public void DisplayPageLeaveWarning() => DisplayPageLeaveWarningCommand.Execute(null);

        public AsyncCommand DisplayPageLeaveWarningCommand
        {
            get => (AsyncCommand) GetValue(DisplayPageLeaveWarningCommandProperty);
            set => SetValue(DisplayPageLeaveWarningCommandProperty, value);
        }

        public void DisplayKeywordReplyPageLeaveWarning() => DisplayKeywordReplyPageLeaveWarningCommand.Execute(null);

        public AsyncCommand DisplayKeywordReplyPageLeaveWarningCommand
        {
            get => (AsyncCommand) GetValue(DisplayKeywordReplyPageLeaveWarningCommandProperty);
            set => SetValue(DisplayKeywordReplyPageLeaveWarningCommandProperty, value);
        }

        public void OnSessionExpiring() => OnSessionExpiringCommand.Execute(null);

        public AsyncCommand OnSessionExpiringCommand
        {
            get => (AsyncCommand) GetValue(OnSessionExpiringCommandProperty);
            set => SetValue(OnSessionExpiringCommandProperty, value);
        }

        public void CreateOnDemandGpSession(string json)
        {
            try
            {
                var request = ConvertFromJsonString<CreateOnDemandGpSessionRequest>(json);
                CreateOnDemandGpSessionCommand.Execute(request);
            }
            catch (ArgumentException e)
            {
                Logger.LogError(e, "Failed to deserialise request to create a gp session");
            }
        }

        public AsyncCommand<CreateOnDemandGpSessionRequest> CreateOnDemandGpSessionCommand
        {
            get => (AsyncCommand<CreateOnDemandGpSessionRequest>) GetValue(CreateOnDemandGpSessionCommandProperty);
            set => SetValue(CreateOnDemandGpSessionCommandProperty, value);
        }

        public async Task SendBiometricStatus(BiometricStatus biometricStatus)
        {
            const string callbackName = "window.nativeAppCallbacks.biometricStatus";
            var jsonArgument = ConvertToJsonString(biometricStatus);
            await EvaluateJavaScriptWithLoggingAsync($"{callbackName}({jsonArgument})").ResumeOnThreadPool();
        }

        public void UpdateBiometricRegistration(string argument) => UpdateBiometricRegistrationCommand.Execute(argument);

        public AsyncCommand<string> UpdateBiometricRegistrationCommand
        {
            get => (AsyncCommand<string>)GetValue(UpdateBiometricRegistrationCommandProperty);
            set => SetValue(UpdateBiometricRegistrationCommandProperty, value);
        }

        public void OpenBrowserOverlay(string argument)
        {
            try
            {
                OpenBrowserOverlayCommand.Execute(new Uri(argument));
            }
            catch (UriFormatException e)
            {
                Logger.LogError(e, "Argument supplied is not a valid Uri: {argument}", argument);
            }
        }

        public AsyncCommand<Uri> OpenBrowserOverlayCommand
        {
            get => (AsyncCommand<Uri>)GetValue(OpenBrowserOverlayCommandProperty);
            set => SetValue(OpenBrowserOverlayCommandProperty, value);
        }

        public void OpenSettings() => OpenSettingsCommand.Execute(null);

        public AsyncCommand OpenSettingsCommand
        {
            get => (AsyncCommand) GetValue(OpenSettingsCommandProperty);
            set => SetValue(OpenSettingsCommandProperty, value);
        }

        public void Logout() => LogoutCommand.Execute(null);

        public AsyncCommand LogoutCommand
        {
            get => (AsyncCommand)GetValue(LogoutCommandProperty);
            set => SetValue(LogoutCommandProperty, value);
        }

        public void SessionExpired() => SessionExpiredCommand.Execute(null);

        public AsyncCommand SessionExpiredCommand
        {
            get => (AsyncCommand)GetValue(SessionExpiredCommandProperty);
            set => SetValue(SessionExpiredCommandProperty, value);
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

        public async Task SendBiometricCompletion(BiometricCompletion biometricCompletion)
        {
            const string callbackName = "window.nativeAppCallbacks.loginSettingsBiometricCompletion";
            var jsonArgument = ConvertToJsonString(biometricCompletion);
            await EvaluateJavaScriptWithLoggingAsync($"{callbackName}({jsonArgument})").ResumeOnThreadPool();
        }

        public async Task SendPageLeave()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.pageLeaveWarningLeavePage()").ResumeOnThreadPool();

        public async Task SendStayOnPage()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.pageLeaveWarningStayOnPage()").ResumeOnThreadPool();

        public async Task SendSessionExtend()
            => await EvaluateJavaScriptWithLoggingAsync("window.nativeAppCallbacks.sessionExtend()").ResumeOnThreadPool();

        public async Task NavigateToAdvice()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.navigationGoToAdvice()").ResumeOnThreadPool();

        public async Task NavigateToAppointments()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.navigationGoToAppointments()").ResumeOnThreadPool();

        public async Task NavigateToPrescriptions()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.navigationGoToPrescriptions()").ResumeOnThreadPool();

        public async Task NavigateToYourHealth()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.navigationGoToYourHealth()").ResumeOnThreadPool();

        public async Task NavigateToMessages()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.navigationGoToMessages()").ResumeOnThreadPool();

        public async Task NavigateToMore()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.navigationGoToMore()").ResumeOnThreadPool();

        public async Task NavigateToHome()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.navigationGoToHome()").ResumeOnThreadPool();

        public async Task AuthLogout()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.authLogout()").ResumeOnThreadPool();

        public async Task NavigateToRedirector(Uri targetUrl)
            => await EvaluateJavaScriptAsync($"window.nativeAppCallbacks.redirectToTargetUrl('{targetUrl}')").ResumeOnThreadPool();

        public async Task NavigateToAppPage(string page)
            => await EvaluateJavaScriptAsync($"window.nativeAppCallbacks.navigateToAppPage('{page}')").ResumeOnThreadPool();

        public async Task ValidateSession()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.validateSession()").ResumeOnThreadPool();

        public async Task AppVersionUpdateNativeVersion(string version)
            => await EvaluateJavaScriptWithLoggingAsync($"window.nativeAppCallbacks.appVersionUpdateNativeVersion('{version}')")
                .ResumeOnThreadPool();

        public async Task NavigateToOnDemandGpReturn(Dictionary<string, string> queryParameters)
        {
            const string callbackName = "window.nativeAppCallbacks.navigateToOnDemandGpReturn";
            var jsonArgument = JsonConvert.SerializeObject(queryParameters);
            await EvaluateJavaScriptAsync($"{callbackName}('{jsonArgument}')").ResumeOnThreadPool();
        }

        public async Task<string> GetCurrentRouteName()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.getCurrentRouteName()").ResumeOnThreadPool();

        public async Task NavigateBack()
        {
            await EvaluateJavaScriptAsync($"window.nativeAppCallbacks.navigationBack()")
                    .ResumeOnThreadPool();
        }

        public async Task GetContextualHelpLink()
            => await EvaluateJavaScriptWithLoggingAsync("window.nativeAppCallbacks.getContextualHelpLink()").ResumeOnThreadPool();

        public void SetBadgeCount(string count) => SetBadgeCountCommand.Execute(count);

        public AsyncCommand<string> SetBadgeCountCommand
        {
            get => (AsyncCommand<string>)GetValue(SetBadgeCountCommandProperty);
            set => SetValue(SetBadgeCountCommandProperty, value);
        }


        private async Task EvaluateJavaScriptWithLoggingAsync(string script)
        {
            try
            {
                await EvaluateJavaScriptAsync(script).ResumeOnThreadPool();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error executing script: {Script}", script);
            }
        }

        private class UriConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Uri);
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                switch (reader)
                {
                    case {TokenType: JsonToken.String}:
                    {
                        if (reader.Value is string url)
                        {
                            return new Uri(url, UriKind.RelativeOrAbsolute);
                        }
                        return null;
                    }
                    case {TokenType: JsonToken.Null}:
                        return null;
                    default:
                        throw new InvalidOperationException("Unhandled case for UriConverter. Check to see if this converter has been applied to the wrong serialization type.");
                }
            }

            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                switch (value)
                {
                    case null:
                        writer.WriteNull();
                        return;
                    case Uri uri:
                        writer.WriteValue(uri.OriginalString);
                        return;
                    default:
                        throw new InvalidOperationException("Unhandled case for UriConverter. Check to see if this converter has been applied to the wrong serialization type.");
                }
            }
        }
    }
}
