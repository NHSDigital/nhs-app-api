using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class NhsLoginPage: INhsLoginView
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<INhsLoginView> _appNavigation;

        public NhsLoginPage(ILogger<NhsLoginPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<INhsLoginView>(this, Navigation);

            InitializeComponent();

            AddEventHandlers();
        }

        IAppNavigation<INhsLoginView> INavigationView<INhsLoginView>.AppNavigation => _appNavigation;

        public Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }
        private AsyncCommand<WebNavigatingEventArgs> NavigatingCommand => new AsyncCommand<WebNavigatingEventArgs>(() => Navigating);

        public Func<Task>? NavigationFailed { get; set; }
        private AsyncCommand NavigationFailedCommand => new AsyncCommand(() => NavigationFailed);
        
        public Func<Task>? BackRequested { get; set; }
        private AsyncCommand BackRequestedCommand => new AsyncCommand(() => BackRequested);

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

        protected override bool OnBackButtonPressed()
        {
            BackRequestedCommand.Execute(null);
            return true;
        }
    }
}
