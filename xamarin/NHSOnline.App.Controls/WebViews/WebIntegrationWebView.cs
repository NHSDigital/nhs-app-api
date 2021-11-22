using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Logging;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.WebViews
{
    public sealed class WebIntegrationWebView : WebView, IRedirectFlowAwareWebView, IPostRequestCapableWebView
    {
        public const string JavascriptObjectName = "nhsappNative";
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(WebIntegrationWebView));
        private static JsonSerializerSettings Settings { get; } = CreateJsonSerializerSettings();

        public static readonly BindableProperty GoToNhsAppPageCommandProperty =
            BindableProperty.Create(
                nameof(GoToNhsAppPageCommand),
                typeof(AsyncCommand<string>),
                typeof(WebIntegrationWebView));

        public static readonly BindableProperty OpenBrowserOverlayCommandProperty =
            BindableProperty.Create(
                nameof(OpenBrowserOverlayCommand),
                typeof(AsyncCommand<Uri>),
                typeof(WebIntegrationWebView));

        public event EventHandler<WebViewPageLoadEventArgs>? PageLoadComplete;

        public void GoToNhsAppPage(string argument) => GoToNhsAppPageCommand.Execute(argument);

        public void OpenBrowserOverlay(string argument)
        {
            try
            {
                OpenBrowserOverlayCommand.Execute(new Uri(argument));
            }
            catch (UriFormatException e)
            {
                Logger.LogError(e, $"Argument supplied is not a valid Uri: {argument}");
            }
        }

        public static readonly BindableProperty AddEventToCalendarCommandProperty =
            BindableProperty.Create(nameof(AddEventToCalendarCommand), typeof(AsyncCommand<AddEventToCalendarRequest>), typeof(WebIntegrationWebView));

        public static readonly BindableProperty StartDownloadCommandProperty =
            BindableProperty.Create(nameof(StartDownloadCommand), typeof(AsyncCommand<DownloadRequest>), typeof(WebIntegrationWebView));

        public static readonly BindableProperty WebIntegrationRequestProperty =
            BindableProperty.Create(nameof(WebIntegrationRequest), typeof(WebIntegrationRequest), typeof(WebIntegrationWebView));

        public AsyncCommand<string> GoToNhsAppPageCommand
        {
            get => (AsyncCommand<string>) GetValue(GoToNhsAppPageCommandProperty);
            set => SetValue(GoToNhsAppPageCommandProperty, value);
        }

        public AsyncCommand<Uri> OpenBrowserOverlayCommand
        {
            get => (AsyncCommand<Uri>) GetValue(OpenBrowserOverlayCommandProperty);
            set => SetValue(OpenBrowserOverlayCommandProperty, value);
        }

        public AsyncCommand<DownloadRequest> StartDownloadCommand
        {
            get => (AsyncCommand<DownloadRequest>) GetValue(StartDownloadCommandProperty);
            set => SetValue(StartDownloadCommandProperty, value);
        }

        public WebIntegrationRequest? WebIntegrationRequest
        {
            get => (WebIntegrationRequest) GetValue(WebIntegrationRequestProperty);
            set => SetValue(WebIntegrationRequestProperty, value);
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

        public void StartDownload(string json)
        {
            var request = ConvertFromJsonString<DownloadRequest>(json);
            StartDownloadCommand.Execute(request);
        }

        public AsyncCommand<AddEventToCalendarRequest> AddEventToCalendarCommand
        {
            get => (AsyncCommand<AddEventToCalendarRequest>) GetValue(AddEventToCalendarCommandProperty);
            set => SetValue(AddEventToCalendarCommandProperty, value);
        }

        private static T ConvertFromJsonString<T>(string json)
            => JsonConvert.DeserializeObject<T>(json, Settings) ?? throw new ArgumentException($"Failed to deserialise JSON to {typeof(T).FullName}", nameof(json));

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

        void IRedirectFlowAwareWebView.OnPageLoadComplete(WebViewPageLoadEventArgs pageLoadEventArgs)
        {
            PageLoadComplete?.Invoke(this, pageLoadEventArgs);
        }
    }
}
