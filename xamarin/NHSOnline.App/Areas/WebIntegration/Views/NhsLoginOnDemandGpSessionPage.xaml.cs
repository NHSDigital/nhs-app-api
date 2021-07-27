using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NHSOnline.App.Areas.WebIntegration.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NhsLoginOnDemandGpSessionPage : INhsLoginOnDemandGpSessionView, INhsLoginOnDemandGpSessionView.IEvents
    {
        private readonly ILogger<NhsLoginOnDemandGpSessionPage> _logger;
        private readonly AppNavigation<INhsLoginOnDemandGpSessionView.IEvents> _appNavigation;

        public NhsLoginOnDemandGpSessionPage(ILogger<NhsLoginOnDemandGpSessionPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<INhsLoginOnDemandGpSessionView.IEvents>(this, Navigation);

            InitializeComponent();

            AddEventHandlers();
        }

        public Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }
        private AsyncCommand<WebNavigatingEventArgs> NavigatingCommand
            => new AsyncCommand<WebNavigatingEventArgs>(() => Navigating);

        public Func<Task>? BackRequested { get; set; }
        private AsyncCommand BackRequestedCommand => new AsyncCommand(() => BackRequested);

        public Func<Task>? NavigationFailed { get; set; }
        private AsyncCommand NavigationFailedCommand => new AsyncCommand(() => NavigationFailed);

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
            NavigatingCommand.Execute(args);
        }

        private void WebViewOnNavigated(object sender, WebNavigatedEventArgs args)
        {
            _logger.LogInformation("Navigated ({Result}): {Uri}", args.Result, args.Url);

            if (args.Result != WebNavigationResult.Success)
            {
                NavigationFailedCommand.Execute(null);
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