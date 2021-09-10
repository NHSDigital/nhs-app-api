using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Events.Models;
using NHSOnline.App.Navigation;
using NHSOnline.App.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Areas.WebIntegration.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NhsLoginOnDemandGpSessionPage : INhsLoginOnDemandGpSessionView, INhsLoginOnDemandGpSessionView.IEvents
    {
        private readonly ILogger<NhsLoginOnDemandGpSessionPage> _logger;
        private readonly AppNavigation<INhsLoginOnDemandGpSessionView.IEvents> _appNavigation;

        private bool OnInitialNavigation { get; set; } = true;
        private Uri? InitialUrl { get; set; }

        public NhsLoginOnDemandGpSessionPage(ILogger<NhsLoginOnDemandGpSessionPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<INhsLoginOnDemandGpSessionView.IEvents>(this, Navigation);

            InitializeComponent();

            AddEventHandlers();
        }

        public Action<WebNavigatingEventArgs>? Navigating { get; set; }

        public Func<NavigationFailedArgs, Task>? NavigationFailed { get; set; }
        private AsyncCommand<NavigationFailedArgs> NavigationFailedCommand
            => new AsyncCommand<NavigationFailedArgs>(() => NavigationFailed);

        public Func<Task>? BackRequested { get; set; }
        private AsyncCommand BackRequestedCommand
            => new AsyncCommand(() => BackRequested);

        IAppNavigation<INhsLoginOnDemandGpSessionView.IEvents> INavigationView<INhsLoginOnDemandGpSessionView.IEvents>.AppNavigation => _appNavigation;

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
            NhsAppResilience.ExecuteImmediately(() => Navigating?.Invoke(args));
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

        public void SetNavigationFooterItem(NavigationFooterItem footerItem) => SelectedNavigationFooterItem = footerItem;

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

        public Func<Uri, Task>? DeeplinkRequested { get; set; }
        public async Task HandleDeeplink(Uri deeplinkUrl)
        {
            if (DeeplinkRequested != null)
            {
                await DeeplinkRequested(deeplinkUrl).PreserveThreadContext();
            }
        }
    }
}