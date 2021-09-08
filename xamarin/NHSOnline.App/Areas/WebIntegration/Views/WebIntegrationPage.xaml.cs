using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Events.Models;
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

        private bool OnInitialNavigation { get; set; } = true;
        private WebIntegrationRequest? InitialRequest { get; set; }

        public WebIntegrationPage(ILogger<WebIntegrationPage> logger)
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

        public Func<NavigationFailedArgs, Task>? NavigationFailed { get; set; }
        private AsyncCommand<NavigationFailedArgs> NavigationFailedCommand
            => new AsyncCommand<NavigationFailedArgs>(() => NavigationFailed);

        public Func<WebIntegrationNavigationFailedArgs, Task>? InitialNavigationFailed { get; set; }
        private AsyncCommand<WebIntegrationNavigationFailedArgs> InitialNavigationFailedCommand
            => new AsyncCommand<WebIntegrationNavigationFailedArgs>(() => InitialNavigationFailed);

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
        }

        private void RemoveEventHandlers()
        {
            WebView.Navigating -= WebViewOnNavigating;
            WebView.Navigated -= WebViewOnNavigated;
        }

        private void WebViewOnNavigating(object sender, WebNavigatingEventArgs args)
        {
            _logger.LogInformation("Navigating: {Uri}", args.Url);
            Navigating?.Invoke(args);
        }

        private void WebViewOnNavigated(object sender, WebNavigatedEventArgs args)
        {
            _logger.LogInformation("Navigated ({Result}): {Uri}", args.Result, args.Url);

            if (args.Result is WebNavigationResult.Success)
            {
                OnInitialNavigation = false;
                WebView.Focus();
            }
            else
            {
                if (InitialRequest == null)
                {
                    _logger.LogError($"{nameof(InitialRequest)} is null but should never be null");

                    HandleNavigationFailed(args);
                }
                else
                {
                    HandleInitialNavigationFailed(InitialRequest);
                }
            }
        }

        private void HandleInitialNavigationFailed(WebIntegrationRequest request)
        {
            WebView.IsVisible = false;
            WebView.WebIntegrationRequest = null;
            InitialNavigationFailedCommand.Execute(new WebIntegrationNavigationFailedArgs(request, OnInitialNavigation));
        }

        private void HandleNavigationFailed(WebNavigatedEventArgs args)
        {
            WebView.IsVisible = false;
            WebView.WebIntegrationRequest = null;
            NavigationFailedCommand.Execute(new NavigationFailedArgs(new Uri(args.Url),OnInitialNavigation));
        }

        public void SetWebIntegrationRequest(WebIntegrationRequest webIntegrationRequest)
        {
            OnInitialNavigation = true;
            InitialRequest = webIntegrationRequest;
            WebView.WebIntegrationRequest = webIntegrationRequest;
        }

        public void SetNavigationFooterItem(NavigationFooterItem footerItem) => SelectedNavigationFooterItem = footerItem;

        private void WebOnEndNavigating(object sender, WebNavigatedEventArgs e)
        {
            Spinner.IsVisible = false;
            WebView.IsVisible = true;
        }

        public async Task HandleDeeplink(Uri deeplinkUrl)
        {
            if (DeepLinkRequested != null)
            {
                await DeepLinkRequested(deeplinkUrl).PreserveThreadContext();
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        public bool ShouldSwipeGoBack()
        {
            return false;
        }
    }
}
