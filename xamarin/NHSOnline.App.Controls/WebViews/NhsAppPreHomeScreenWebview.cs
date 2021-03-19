using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Threading;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.WebViews
{
    public sealed class NhsAppPreHomeScreenWebview: WebView
    {
        public static readonly BindableProperty GetNotificationsStatusCommandProperty =
            BindableProperty.Create(nameof(GetNotificationsStatusCommand), typeof(AsyncCommand), typeof(NhsAppPreHomeScreenWebview));

        public static readonly BindableProperty RequestPnsTokenCommandProperty =
            BindableProperty.Create(nameof(RequestPnsTokenCommand), typeof(AsyncCommand<string>), typeof(NhsAppPreHomeScreenWebview));

        public static readonly BindableProperty GoToLoggedInHomeScreenCommandProperty =
            BindableProperty.Create(nameof(GoToLoggedInHomeScreenCommand), typeof(AsyncCommand), typeof(NhsAppPreHomeScreenWebview));

        private static JsonSerializerSettings Settings { get; } = CreateJsonSerializerSettings();

        public Func<Cookie, Task>? SetCookie { get; set; }

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
            const string? callbackName = "window.nativeAppCallbacks.notificationsAuthorised";
            var argumentJson = ConvertToJsonString(authorisedResponse);
            await EvaluateJavaScriptAsync($"{callbackName}({argumentJson})").ResumeOnThreadPool();
        }

        public async Task SendNotificationUnauthorised()
            => await EvaluateJavaScriptAsync("window.nativeAppCallbacks.notificationsUnauthorised()").ResumeOnThreadPool();

        public void GoToLoggedInHomeScreen() => GoToLoggedInHomeScreenCommand.Execute(null);

        public AsyncCommand GoToLoggedInHomeScreenCommand
        {
            get => (AsyncCommand) GetValue(GoToLoggedInHomeScreenCommandProperty);
            set => SetValue(GoToLoggedInHomeScreenCommandProperty, value);
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
