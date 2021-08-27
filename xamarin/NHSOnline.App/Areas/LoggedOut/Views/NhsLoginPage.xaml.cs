using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Events.Models;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class NhsLoginPage: INhsLoginView, INhsLoginView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<INhsLoginView.IEvents> _appNavigation;

        private bool OnInitialNavigation { get; set; } = true;
        private Uri? InitialUrl { get; set; }

        public NhsLoginPage(ILogger<NhsLoginPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<INhsLoginView.IEvents>(this, Navigation);

            InitializeComponent();

            AddEventHandlers();
        }

        IAppNavigation<INhsLoginView.IEvents> INavigationView<INhsLoginView.IEvents>.AppNavigation => _appNavigation;

        public Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }
        private AsyncCommand<WebNavigatingEventArgs> NavigatingCommand
            => new AsyncCommand<WebNavigatingEventArgs>(() => Navigating);

        public Func<NavigationFailedArgs, Task>? NavigationFailed { get; set; }
        private AsyncCommand<NavigationFailedArgs> NavigationFailedCommand
            => new AsyncCommand<NavigationFailedArgs>(() => NavigationFailed);

        public Func<Task>? BackRequested { get; set; }
        private AsyncCommand BackRequestedCommand
            => new AsyncCommand(() => BackRequested);

        public Func<Uri, Task>? DeeplinkRequested { get; set; }

        protected override void OnAppearing()
        {
            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();

            RemoveEventHandlers();
            AddEventHandlers();
        }

        protected override void OnDisappearing()
        {
            _logger.LogInformation("{Method}", nameof(OnDisappearing));
            _appNavigation.SuppressHandlers();

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
            NavigatingCommand.Execute(args);
        }

        private void WebViewOnNavigated(object sender, WebNavigatedEventArgs args)
        {
            _logger.LogInformation("Navigated ({Result}): {Uri}", args.Result, args.Url);

            if (args.Result is WebNavigationResult.Success)
            {
                OnInitialNavigation = false;
            }
            else if (InitialUrl is null)
            {
                _logger.LogError($"{nameof(InitialUrl)} is null but should never be null");

                WebView.IsVisible = false;
                NavigationFailedCommand.Execute(new NavigationFailedArgs(new Uri(args.Url), OnInitialNavigation));
            }
            else
            {
                WebView.IsVisible = false;
                NavigationFailedCommand.Execute(new NavigationFailedArgs(InitialUrl, OnInitialNavigation));
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
            GoToUri(uri);
        }

        public void GoToUri(Uri uri)
        {
            OnInitialNavigation = true;
            InitialUrl = uri;

            WebView.GoToUri(uri);
        }

        protected override bool OnBackButtonPressed()
        {
            BackRequestedCommand.Execute(null);
            return true;
        }

        private void WebViewNavigating (object sender, WebNavigatingEventArgs e)
        {
            Spinner.IsVisible = true;
            WebView.IsVisible = false;
        }

        private void WebOnEndNavigating (object sender, WebNavigatedEventArgs e)
        {
            Spinner.IsVisible = false;
            WebView.IsVisible = true;
        }

        public async Task HandleDeeplink(Uri deeplinkUrl)
        {
            if (DeeplinkRequested != null)
            {
                await DeeplinkRequested(deeplinkUrl).PreserveThreadContext();
            }
        }
    }
}
