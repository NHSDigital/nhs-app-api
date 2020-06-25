using System;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class NhsLoginPage: INhsLoginView
    {
        private readonly ILogger _logger;

        public NhsLoginPage(ILogger<NhsLoginPage> logger)
        {
            _logger = logger;

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            WebView.Navigating += WebViewOnNavigating;
            WebView.Navigated += WebViewOnNavigated;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

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

        public void LoadUrlAndNotifyOnRedirect(Uri uri, Func<Uri, bool> isRedirect, Action<Uri> redirected)
        {
            void OnWebViewOnNavigating(object sender, WebNavigatingEventArgs args)
            {
                var redirectedUri = new Uri(args.Url);
                if (isRedirect(redirectedUri))
                {
                    args.Cancel = true;
                    WebView.Navigating -= OnWebViewOnNavigating;
                    redirected(redirectedUri);
                }
            }

            WebView.Navigating += OnWebViewOnNavigating;
            WebView.Source = new UrlWebViewSource { Url = uri.ToString() };
        }
    }
}
