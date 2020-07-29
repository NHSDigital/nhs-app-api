using System;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls.WebViews;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.ThirdParty.Views
{
    [DesignTimeVisible(false)]
    public partial class NhsAppSilverWebPage : INhsAppSilverWebView
    {
        private readonly ILogger _logger;

        public NhsAppSilverWebPage(ILogger<NhsAppSilverWebPage> logger)
        {
            _logger = logger;

            InitializeComponent();

            AddEventHandlers();

            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            RemoveEventHandlers();
            AddEventHandlers();
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

        public void GoToUri(Uri uri) => WebView.GoToUri(uri);

        // This will be changed in NHSO-10645 when we update with web native changes
        public void NavigateWithinApp(string spaPath) => WebView.EvaluateJavaScriptAsync($"window.$nuxt.$store.dispatch('navigation/goTo', '{spaPath}')");
    }
}
