using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.DependencyServices.Navigation;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration.Views
{
    [DesignTimeVisible(false)]
    public partial class NhsLoginUpliftPage : INhsLoginUpliftView, INhsLoginUpliftView.IEvents
    {
        public event EventHandler<FocusRequestArgs>? AccessibilityFocusChangeRequested;
        private const string PageName = "NHS Login";

        private readonly ILogger _logger;
        private readonly AppNavigation<INhsLoginUpliftView.IEvents> _appNavigation;

        public NhsLoginUpliftPage(
            ILogger<NhsLoginUpliftPage> logger,
            IAccessibilityService accessibilityService,
            INavigationService navigationService): base(accessibilityService)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<INhsLoginUpliftView.IEvents>(this, navigationService);

            InitializeComponent();

            AddEventHandlers();
        }

        IAppNavigation<INhsLoginUpliftView.IEvents> INavigationView<INhsLoginUpliftView.IEvents>.AppNavigation =>
            _appNavigation;

        public Action<WebNavigatingEventArgs>? Navigating { get; set; }

        public Func<Task>? NavigationFailed { get; set; }
        private AsyncCommand NavigationFailedCommand
            => new AsyncCommand(() => NavigationFailed);

        public Func<Task>? BackRequested { get; set; }
        private AsyncCommand BackRequestedCommand
            => new AsyncCommand(() => BackRequested);

        public Func<ISelectMediaRequest, Task>? SelectMediaRequested { get; set; }
        public AsyncCommand<ISelectMediaRequest> SelectMediaCommand
            => new AsyncCommand<ISelectMediaRequest>(() => SelectMediaRequested);

        protected override void OnAppearing()
        {
            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();

            RemoveEventHandlers();
            AddEventHandlers();

            base.OnAppearing();
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
            NhsAppResilience.ExecuteImmediately(() => Navigating?.Invoke(args));
            if (args.Cancel)
            {
                ShowWebView();
            }
        }

        private void WebViewOnNavigated(object sender, WebNavigatedEventArgs args)
        {
            _logger.LogInformation("Navigated ({Result}): {Uri}", args.Result, args.Url);

            if (args.Result is WebNavigationResult.Cancel)
            {
                _logger.LogInformation("Web navigation was cancelled");
            }
            else if(args.Result != WebNavigationResult.Success)
            {
                WebView.IsVisible = false;
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
            GoToUri(uri);
        }

        private void ShowWebView()
        {
            Spinner.IsVisible = false;
            WebView.IsVisible = true;
            AccessibilityFocus();
        }

        private void ShowSpinner()
        {
            Spinner.IsVisible = false;
            Spinner.AccessibilityFocus();
            WebView.IsVisible = true;
        }

        public void GoToUri(Uri uri) => WebView.GoToUri(uri);

        protected override bool OnBackButtonPressed()
        {
            BackRequestedCommand.Execute(null);
            return true;
        }

        public Task HandleDeeplink(Uri deeplinkUrl)
        {
            _logger.LogInformation("{className} is not required to handle deeplinks", nameof(NhsLoginUpliftPage));
            return Task.CompletedTask;
        }

        private void WebViewNavigating(object sender, WebNavigatingEventArgs e)
        {
            ShowSpinner();
        }

        private void WebOnEndNavigating(object sender, WebNavigatedEventArgs e)
        {
            ShowWebView();
            PageDescription = PageName;
            AnnouncePage();
        }

        private void AccessibilityFocus()
        {
            if (AccessibilityFocusChangeRequested != null)
            {
                var arg = new FocusRequestArgs {Focus = true};
                AccessibilityFocusChangeRequested(this, arg);
            }
        }
    }
}