using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.WebViews
{
    public sealed class WebIntegrationWebView : WebViewBase, IRedirectFlowAwareWebView, IPostRequestCapableWebView
    {
        public const string JavascriptObjectName = "nhsappNative";
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(WebIntegrationWebView));

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

        public static readonly BindableProperty OpenExternalBrowserCommandProperty =
            BindableProperty.Create(
                nameof(OpenExternalBrowserCommand),
                typeof(AsyncCommand<string>),
                typeof(WebIntegrationWebView));

        public static readonly BindableProperty SslErrorCommandProperty =
            BindableProperty.Create(
                nameof(SslErrorCommand),
                typeof(AsyncCommand<SslErrorDetails>),
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
                Logger.LogError(e, "Argument supplied is not a valid Uri: {Argument}", argument);
            }
        }

        public void OpenExternalBrowser(string argument)
        {
            try
            {
                OpenExternalBrowserCommand.Execute(argument);
            }
            catch (UriFormatException e)
            {
                Logger.LogError(e, "Argument supplied is not a valid Uri: {Argument}", argument);
            }
        }

        public static readonly BindableProperty AddEventToCalendarCommandProperty =
            BindableProperty.Create(nameof(AddEventToCalendarCommand), typeof(AsyncCommand<AddEventToCalendarRequest>), typeof(WebIntegrationWebView));

        public static readonly BindableProperty StartDownloadCommandProperty =
            BindableProperty.Create(nameof(StartDownloadCommand), typeof(AsyncCommand<DownloadRequest>), typeof(WebIntegrationWebView));

        public static readonly BindableProperty WebIntegrationRequestProperty =
            BindableProperty.Create(nameof(WebIntegrationRequest), typeof(WebIntegrationRequest), typeof(WebIntegrationWebView));

        public static readonly BindableProperty BackCommandProperty =
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

        public AsyncCommand<string> OpenExternalBrowserCommand
        {
            get => (AsyncCommand<string>) GetValue(OpenExternalBrowserCommandProperty);
            set => SetValue(OpenExternalBrowserCommandProperty, value);
        }

        public AsyncCommand<DownloadRequest> StartDownloadCommand
        {
            get => (AsyncCommand<DownloadRequest>) GetValue(StartDownloadCommandProperty);
            set => SetValue(StartDownloadCommandProperty, value);
        }

        public AsyncCommand<SslErrorDetails> SslErrorCommand
        {
            get => (AsyncCommand<SslErrorDetails>) GetValue(SslErrorCommandProperty);
            set => SetValue(SslErrorCommandProperty, value);
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

        public AsyncCommand BackCommand
        {
            get => (AsyncCommand) GetValue(BackCommandProperty);
            set => SetValue(BackCommandProperty, value);
        }

        void IRedirectFlowAwareWebView.OnPageLoadComplete(WebViewPageLoadEventArgs pageLoadEventArgs)
        {
            PageLoadComplete?.Invoke(this, pageLoadEventArgs);
        }

        public void OnSslError(SslErrorDetails sslErrorDetails)
        {
            SslErrorCommand.Execute(sslErrorDetails);
        }

        public async Task NavigateBack()
        {
            await EvaluateJavaScriptAsync("window.nhsapp?.callbacks?.back()")
                .ResumeOnThreadPool();
        }
    }
}
