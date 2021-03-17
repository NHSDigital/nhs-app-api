using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration.Views
{
    [DesignTimeVisible(false)]
    public partial class WebIntegrationPage : IWebIntegrationView, IWebIntegrationView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<IWebIntegrationView.IEvents> _appNavigation;

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
        private AsyncCommand AppearingCommand => new AsyncCommand(() => ((IWebIntegrationView.IEvents)this).Appearing);

        public Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }
        private AsyncCommand<WebNavigatingEventArgs> NavigatingCommand => new AsyncCommand<WebNavigatingEventArgs>(() => Navigating);

        public Func<string, Task>? RedirectToNhsAppPageRequested { get; set; }
        public AsyncCommand<string> RedirectToNhsAppPageCommand
            => new AsyncCommand<string>(() => RedirectToNhsAppPageRequested);

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
            NavigatingCommand.Execute(args);
        }

        private void WebViewOnNavigated(object sender, WebNavigatedEventArgs args)
        {
            _logger.LogInformation("Navigated ({Result}): {Uri}", args.Result, args.Url);
        }

        public void GoToUri(Uri uri) => WebView.GoToUri(uri);
    }
}
