using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.WebViews
{
    public sealed class NhsAppWebView: WebView, IAccessibleControl
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

        public static readonly BindableProperty OnSessionExpiringCommandProperty =
            BindableProperty.Create(nameof(OnSessionExpiringCommandProperty), typeof(AsyncCommand), typeof(NhsAppWebView));

        public static readonly BindableProperty LogoutCommandProperty =
            BindableProperty.Create(nameof(LogoutCommand), typeof(AsyncCommand), typeof(NhsAppWebView));

        public static readonly BindableProperty SessionExpiredCommandProperty =
            BindableProperty.Create(nameof(SessionExpiredCommand), typeof(AsyncCommand), typeof(NhsAppWebView));

        public static readonly BindableProperty CreateOnDemandGpSessionCommandProperty =
            BindableProperty.Create(nameof(CreateOnDemandGpSessionCommand), typeof(AsyncCommand<CreateOnDemandGpSessionRequest>), typeof(NhsAppWebView));

        private static JsonSerializerSettings Settings { get; } = CreateJsonSerializerSettings();

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
            await EvaluateJavaScriptAsync($"{callbackName}({argumentJson})").ResumeOnThreadPool();
        }

        public async Task SendNotificationAuthorised(NotificationAuthorisedResponse authorisedResponse)
        {
            const string callbackName = "window.nativeAppCallbacks.notificationsAuthorised";
            var argumentJson = ConvertToJsonString(authorisedResponse);
            await EvaluateJavaScriptAsync($"{callbackName}({argumentJson})").ResumeOnThreadPool();
        }

        public async Task SendNotificationUnauthorised()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.notificationsUnauthorised()").ResumeOnThreadPool();

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
            await EvaluateJavaScriptAsync($"{callbackName}({jsonArgument})").ResumeOnThreadPool();
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

        public async Task SendBiometricCompletion(BiometricCompletion biometricCompletion)
        {
            const string callbackName = "window.nativeAppCallbacks.loginSettingsBiometricCompletion";
            var jsonArgument = ConvertToJsonString(biometricCompletion);
            await EvaluateJavaScriptAsync($"{callbackName}({jsonArgument})").ResumeOnThreadPool();
        }

        public async Task SendPageLeave()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.pageLeaveWarningLeavePage()").ResumeOnThreadPool();

        public async Task SendStayOnPage()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.pageLeaveWarningStayOnPage()").ResumeOnThreadPool();

        public async Task SendSessionExtend()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.sessionExtend()").ResumeOnThreadPool();

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
            => await EvaluateJavaScriptAsync($"window.nativeAppCallbacks.appVersionUpdateNativeVersion('{version}')")
                .ResumeOnThreadPool();

        public async Task NavigateToOnDemandGpReturn(Dictionary<string, string> queryParameters)
        {
            const string callbackName = "window.nativeAppCallbacks.navigateToOnDemandGpReturn";
            var jsonArgument = JsonConvert.SerializeObject(queryParameters);
            await EvaluateJavaScriptAsync($"{callbackName}('{jsonArgument}')").ResumeOnThreadPool();
        }

        public async Task<string> GetLastCrumbIfExists()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.retrieveLastCrumbName()").ResumeOnThreadPool();

        public async Task NavigateBack()
        {
            await EvaluateJavaScriptAsync($"window.nativeAppCallbacks.navigationBack()")
                    .ResumeOnThreadPool();
        }

        public async Task GetContextualHelpLink()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.getContextualHelpLink()").ResumeOnThreadPool();

        private static T ConvertFromJsonString<T>(string json)
            => JsonConvert.DeserializeObject<T>(json, Settings) ?? throw new ArgumentException($"Failed to deserialise JSON to {typeof(T).FullName}", nameof(json));

        private static string ConvertToJsonString<T>(T value) => JsonConvert.SerializeObject(value, Settings);

        private static JsonSerializerSettings CreateJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
            settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
            settings.Converters.Add(new UriConverter());
            return settings;
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
