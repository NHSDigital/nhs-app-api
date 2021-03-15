using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.PreHome.Views
{
    [DesignTimeVisible(false)]
    public partial class NhsAppPreHomeScreenWebPage : INhsAppPreHomeScreenWebView, INhsAppPreHomeScreenWebView.IEvents, IRootPage
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<INhsAppPreHomeScreenWebView.IEvents> _appNavigation;

        public NhsAppPreHomeScreenWebPage(ILogger<NhsAppPreHomeScreenWebPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<INhsAppPreHomeScreenWebView.IEvents>(this, Navigation);

            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        IAppNavigation<INhsAppPreHomeScreenWebView.IEvents> INavigationView<INhsAppPreHomeScreenWebView.IEvents>.AppNavigation => _appNavigation;

        Func<Task>? INhsAppPreHomeScreenWebView.Appearing { get; set; }

        public Func<Task>? ResetAndShowErrorRequested { get; set; }

        public Func<Task>? GetNotificationsStatusRequested { get; set; }

        public Func<Task>? GoToLoggedInHomeRequested { get; set; }

        private AsyncCommand AppearingCommand => new AsyncCommand(() => ((INhsAppPreHomeScreenWebView)this).Appearing);

        public Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }
        private AsyncCommand<WebNavigatingEventArgs> NavigatingCommand => new AsyncCommand<WebNavigatingEventArgs>(() => Navigating);

        public Func<WebNavigatedEventArgs, Task>? Navigated { get; set; }
        private AsyncCommand<WebNavigatedEventArgs> NavigatedCommand => new AsyncCommand<WebNavigatedEventArgs>(() => Navigated);

        public AsyncCommand GetNotificationsStatusCommand
            => new AsyncCommand(() => GetNotificationsStatusRequested);

        public AsyncCommand GoToLoggedInHomeCommand
            => new AsyncCommand(() => GoToLoggedInHomeRequested);

        protected override void OnAppearing()
        {
            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();

            RemoveEventHandlers();
            AddEventHandlers();

            AppearingCommand.Execute(null);
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
            => NavigatingCommand.Execute(args);

        private void WebViewOnNavigated(object sender, WebNavigatedEventArgs args)
            => NavigatedCommand.Execute(args);

        public async Task AddCookie(Cookie cookie)
            => await (WebView.SetCookie?.Invoke(cookie) ?? Task.CompletedTask).PreserveThreadContext();

        public void GoToUri(Uri uri) => WebView.GoToUri(uri);

        public async Task ResetAndShowError()
            => await (ResetAndShowErrorRequested?.Invoke() ?? Task.CompletedTask).PreserveThreadContext();

        public async Task SendNotificationsStatus(string status)
            => await WebView.EvaluateJavaScriptAsync($"window.nativeAppCallbacks.notificationsSettingsStatus({ConvertToJsonString(status)})").PreserveThreadContext();

        private static string ConvertToJsonString(string arg)
        {
            return JsonConvert.SerializeObject(arg);
        }
    }
}