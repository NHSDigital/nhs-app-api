using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NHSOnline.App.Areas.Home;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Threading;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.WebViews
{
    public sealed class NhsAppWebView: WebView
    {
        public static readonly BindableProperty OpenWebIntegrationCommandProperty =
            BindableProperty.Create(nameof(OpenWebIntegrationCommand), typeof(AsyncCommand<OpenWebIntegrationRequest>), typeof(NhsAppWebView));

        public static readonly BindableProperty StartNhsLoginUpliftCommandProperty =
            BindableProperty.Create(nameof(StartNhsLoginUpliftCommand), typeof(AsyncCommand<StartNhsLoginUpliftRequest>), typeof(NhsAppWebView));

        public static readonly BindableProperty GetNotificationsStatusCommandProperty =
            BindableProperty.Create(nameof(GetNotificationsStatusCommand), typeof(AsyncCommand), typeof(NhsAppWebView));

        public static readonly BindableProperty RequestPnsTokenCommandProperty =
            BindableProperty.Create(nameof(RequestPnsTokenCommand), typeof(AsyncCommand<string>), typeof(NhsAppPreHomeScreenWebview));

        public static readonly BindableProperty FetchBiometricSpecCommandProperty =
            BindableProperty.Create(nameof(FetchBiometricSpecCommand), typeof(AsyncCommand), typeof(NhsAppWebView));

        public static readonly BindableProperty UpdateBiometricRegistrationCommandProperty =
            BindableProperty.Create(nameof(UpdateBiometricRegistrationCommand), typeof(AsyncCommand<string>), typeof(NhsAppWebView));

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

        public void RequestPnsToken(string argument) => RequestPnsTokenCommand.Execute(argument);

        public async Task SendNotificationAuthorised(NotificationAuthorisedResponse authorisedResponse)
        {
            const string? callbackName = "window.nativeAppCallbacks.notificationsAuthorised";
            var argumentJson = ConvertToJsonString(authorisedResponse);
            await EvaluateJavaScriptAsync($"{callbackName}({argumentJson})").ResumeOnThreadPool();
        }

        public async Task SendNotificationUnauthorised()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.notificationsUnauthorised()").ResumeOnThreadPool();

        public AsyncCommand<string> RequestPnsTokenCommand
        {
            get => (AsyncCommand<string>) GetValue(RequestPnsTokenCommandProperty);
            set => SetValue(RequestPnsTokenCommandProperty, value);
        }

        public void FetchBiometricSpec() => FetchBiometricSpecCommand.Execute(null);

        public AsyncCommand FetchBiometricSpecCommand
        {
            get => (AsyncCommand)GetValue(FetchBiometricSpecCommandProperty);
            set => SetValue(FetchBiometricSpecCommandProperty, value);
        }

        public async Task SendBiometricSpec(BiometricSpec biometricSpec)
            => await EvaluateJavaScriptAsync($"window.nativeAppCallbacks.loginSettingsBiometricSpec({ConvertToJsonString(biometricSpec)})").ResumeOnThreadPool();

        public void UpdateBiometricRegistration(string argument) => UpdateBiometricRegistrationCommand.Execute(argument);

        public AsyncCommand<string> UpdateBiometricRegistrationCommand
        {
            get => (AsyncCommand<string>)GetValue(UpdateBiometricRegistrationCommandProperty);
            set => SetValue(UpdateBiometricRegistrationCommandProperty, value);
        }

        public async Task SendBiometricCompletion(BiometricCompletion biometricCompletion)
        {
            const string callbackName = "window.nativeAppCallbacks.loginSettingsBiometricCompletion";
            var jsonArgument = ConvertToJsonString(biometricCompletion);
            await EvaluateJavaScriptAsync($"{callbackName}({jsonArgument})").ResumeOnThreadPool();
        }

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

        public async Task NavigateToSettings()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.navigationGoToSettings()").ResumeOnThreadPool();

        public async Task NavigateToHome()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.navigationGoToHome()").ResumeOnThreadPool();

        public async Task NavigateToRedirectedPathWithinApp(string spaPath)
        {
            var callbackName = "window.nativeAppCallbacks.navigationGoTo";
            var argumentJson = ConvertToJsonString(spaPath);
            await EvaluateJavaScriptAsync($"{callbackName}({argumentJson})").ResumeOnThreadPool();
        }

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
            return settings;
        }
    }
}
