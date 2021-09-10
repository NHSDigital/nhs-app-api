using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Events.Models;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration.Views
{
    [DesignTimeVisible(false)]
    public partial class NhsLoginUpliftPage : INhsLoginUpliftView, INhsLoginUpliftView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<INhsLoginUpliftView.IEvents> _appNavigation;

        private bool OnInitialNavigation { get; set; } = true;
        private Uri? InitialUrl { get; set; }

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

        public Func<NavigationFailedArgs, Task>? NavigationFailed { get; set; }
        private AsyncCommand<NavigationFailedArgs> NavigationFailedCommand
            => new AsyncCommand<NavigationFailedArgs>(() => NavigationFailed);

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

        protected override bool OnBackButtonPressed()
        {
            BackRequestedCommand.Execute(null);
            return true;
        }

        public void GoToUri(Uri uri)
        {
            OnInitialNavigation = true;
            InitialUrl = uri;

            WebView.GoToUri(uri);
        }

        public Task HandleDeeplink(Uri deeplinkUrl)
        {
            _logger.LogInformation("{className} is not required to handle deeplinks", nameof(NhsLoginUpliftPage));
            return Task.CompletedTask;
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
    }
}
