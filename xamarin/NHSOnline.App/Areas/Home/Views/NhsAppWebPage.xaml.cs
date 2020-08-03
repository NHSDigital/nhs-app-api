using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Controls.WebViews.KnownServices;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.Home.Views
{
    [DesignTimeVisible(false)]
    public partial class NhsAppWebPage : INhsAppWebView, IRootPage
    {
        private readonly ILogger _logger;

        public NhsAppWebPage(ILogger<NhsAppWebPage> logger)
        {
            _logger = logger;

            InitializeComponent();

            AddEventHandlers();

            NavigationPage.SetHasNavigationBar(this, false);
        }

        Func<Task>? INhsAppWebView.Appearing { get; set; }

        public Func<OpenWebIntegrationRequest, Task>? OpenWebIntegrationRequested { get; set; }

        public Func<Task>? ResetAndShowErrorRequested { get; set; }

        public AsyncCommand<OpenWebIntegrationRequest> OpenWebIntegrationCommand
            => new AsyncCommand<OpenWebIntegrationRequest>(() => OpenWebIntegrationRequested);

        private AsyncCommand AppearingCommand => new AsyncCommand(() => ((INhsAppWebView)this).Appearing);

        protected override void OnAppearing()
        {
            base.OnAppearing();

            RemoveEventHandlers();
            AddEventHandlers();

            AppearingCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

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
        }

        private void WebViewOnNavigated(object sender, WebNavigatedEventArgs args)
        {
            _logger.LogInformation("Navigated ({Result}): {Uri}", args.Result, args.Url);
        }

        public async Task AddCookie(Cookie cookie) =>
            await (WebView.SetCookie?.Invoke(cookie) ?? Task.CompletedTask).PreserveThreadContext();

        public void GoToUri(Uri uri) =>
            WebView.GoToUri(uri);

        // This will be changed in NHSO-10645 when we update with web native changes
        public async Task NavigateWithinApp(string spaPath) =>
            await WebView.EvaluateJavaScriptAsync($"window.$nuxt.$store.dispatch('navigation/goTo', '{spaPath}')").PreserveThreadContext();

        public async Task ResetAndShowError() =>
            await (ResetAndShowErrorRequested?.Invoke() ?? Task.CompletedTask).PreserveThreadContext();
    }
}
