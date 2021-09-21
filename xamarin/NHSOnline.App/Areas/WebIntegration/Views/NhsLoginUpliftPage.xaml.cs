using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration.Views
{
    [DesignTimeVisible(false)]
    public partial class NhsLoginUpliftPage : INhsLoginUpliftView, INhsLoginUpliftView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<INhsLoginUpliftView.IEvents> _appNavigation;

        public NhsLoginUpliftPage(ILogger<NhsLoginUpliftPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<INhsLoginUpliftView.IEvents>(this, Navigation);

            InitializeComponent();

            AddEventHandlers();
        }

        IAppNavigation<INhsLoginUpliftView.IEvents> INavigationView<INhsLoginUpliftView.IEvents>.AppNavigation => _appNavigation;

        Func<Task>? INhsLoginUpliftView.IEvents.Appearing { get; set; }
        private AsyncCommand AppearingCommand
            => new AsyncCommand(() => ((INhsLoginUpliftView.IEvents)this).Appearing);

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

        protected override bool OnBackButtonPressed()
        {
            BackRequestedCommand.Execute(null);
            return true;
        }

        private void ShowWebView()
        {
            Spinner.IsVisible = false;
            WebView.IsVisible = true;
        }

        private void ShowSpinner()
        {
            Spinner.IsVisible = false;
            WebView.IsVisible = true;
        }

        public void GoToUri(Uri uri) => WebView.GoToUri(uri);

        public Task HandleDeeplink(Uri deeplinkUrl)
        {
            _logger.LogInformation("{className} is not required to handle deeplinks", nameof(NhsLoginUpliftPage));
            return Task.CompletedTask;
        }

        private void WebViewNavigating (object sender, WebNavigatingEventArgs e)
        {
            ShowSpinner();
        }

        private void WebOnEndNavigating (object sender, WebNavigatedEventArgs e)
        {
            ShowWebView();
        }
    }
}
