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

        public event EventHandler<EventArgs>? NavigationFailed;

        public NhsLoginPage(ILogger<NhsLoginPage> logger)
        {
            _logger = logger;

            InitializeComponent();

            AddEventHandlers();
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

            if (args.Result != WebNavigationResult.Success)
            {
                NavigationFailed?.Invoke(this, EventArgs.Empty);
            }
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
