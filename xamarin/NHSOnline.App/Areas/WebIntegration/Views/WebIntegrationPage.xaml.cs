using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Navigation;
using NHSOnline.App.Threading;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration.Views
{
    [DesignTimeVisible(false)]
    public partial class WebIntegrationPage : IWebIntegrationView, IWebIntegrationView.IEvents, ISwipeablePage
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<IWebIntegrationView.IEvents> _appNavigation;

        public WebIntegrationPage(ILogger<WebIntegrationPage> logger, IAccessibilityService accessibilityService): base(accessibilityService)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<IWebIntegrationView.IEvents>(this, Navigation);

            InitializeComponent();

            AddEventHandlers();

            NavigationPage.SetHasNavigationBar(this, false);
        }

        IAppNavigation<IWebIntegrationView.IEvents> INavigationView<IWebIntegrationView.IEvents>.AppNavigation => _appNavigation;

        Func<Task>? IWebIntegrationView.IEvents.Appearing { get; set; }
        private AsyncCommand AppearingCommand => new AsyncCommand(()
            => ((IWebIntegrationView.IEvents)this).Appearing);

        public Action<WebNavigatingEventArgs>? Navigating { get; set; }

        public Func<Task>? NavigationFailed { get; set; }
        private AsyncCommand NavigationFailedCommand
            => new AsyncCommand(() => NavigationFailed);

        public Func<string, Task>? GoToNhsAppPageRequested { get; set; }
        public AsyncCommand<string> GoToNhsAppPageCommand
            => new AsyncCommand<string>(() => GoToNhsAppPageRequested);

        public Func<AddEventToCalendarRequest, Task>? AddEventToCalendarRequested { get; set; }
        public AsyncCommand<AddEventToCalendarRequest> AddEventToCalendarCommand
            => new AsyncCommand<AddEventToCalendarRequest>(() => AddEventToCalendarRequested);

        public Func<DownloadRequest, Task>? StartDownloadRequested { get; set; }
        public AsyncCommand<DownloadRequest> StartDownloadCommand
            => new AsyncCommand<DownloadRequest>(() => StartDownloadRequested);

        public Func<Uri, Task>? DeepLinkRequested { get; set; }

        public Action<WebViewPageLoadEventArgs>? PageLoadComplete { get; set; }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();

            RemoveEventHandlers();
            AddEventHandlers();

            AppearingCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _logger.LogInformation("{Method}", nameof(OnDisappearing));
            _appNavigation.SuppressHandlers();

            RemoveEventHandlers();
        }

        private void AddEventHandlers()
        {
            WebView.Navigating += WebViewOnNavigating;
            WebView.Navigated += WebViewOnNavigated;
            WebView.PageLoadComplete += OnPageLoadComplete;
        }

        private void RemoveEventHandlers()
        {
            WebView.Navigating -= WebViewOnNavigating;
            WebView.Navigated -= WebViewOnNavigated;
            WebView.PageLoadComplete -= OnPageLoadComplete;
        }

        private void WebViewOnNavigating(object sender, WebNavigatingEventArgs args)
        {
            _logger.LogInformation("Navigating: {Uri}", args.Url);
            NhsAppResilience.ExecuteImmediately(() => Navigating?.Invoke(args));
        }

        private void WebViewOnNavigated(object sender, WebNavigatedEventArgs args)
        {
            _logger.LogInformation("Navigated ({Result}): {Uri}", args.Result, args.Url);

            if (args.Result != WebNavigationResult.Success && args.Result != WebNavigationResult.Cancel)
            {
                WebView.IsVisible = false;
                WebView.WebIntegrationRequest = null;
                NavigationFailedCommand.Execute(null);
            }
            else if (args.Result is WebNavigationResult.Cancel)
            {
                _logger.LogInformation("Web navigation was cancelled");
            }
        }

        public void SetWebIntegrationRequest(WebIntegrationRequest webIntegrationRequest) => WebView.WebIntegrationRequest = webIntegrationRequest;

        public void SetNavigationFooterItem(NavigationFooterItem footerItem) => SelectedNavigationFooterItem = footerItem;
        public View GetWebViewElement() => WebView;

        private void WebOnEndNavigating(object sender, WebNavigatedEventArgs e)
        {
            Spinner.IsVisible = false;
            WebView.IsVisible = true;
        }

        private void OnPageLoadComplete(object sender, WebViewPageLoadEventArgs pageLoadEventArgs)
        {
            if (PageLoadComplete != null)
            {
                NhsAppResilience.ExecuteOnMainThread(() => PageLoadComplete.Invoke(pageLoadEventArgs));
            }
        }

        public async Task HandleDeeplink(Uri deeplinkUrl)
        {
            if (DeepLinkRequested != null)
            {
                await DeepLinkRequested(deeplinkUrl).PreserveThreadContext();
            }
        }

        protected override bool OnBackButtonPressed() => true;

        public bool OnSwipeBack() => true;
    }
}
