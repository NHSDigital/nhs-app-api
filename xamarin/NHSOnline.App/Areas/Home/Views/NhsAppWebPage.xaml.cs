using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Controls.WebViews.Payloads;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.Home.Views
{
    [DesignTimeVisible(false)]
    public partial class NhsAppWebPage : INhsAppWebView, IRootPage
    {
        public NhsAppWebPage()
        {
            InitializeComponent();

            AddEventHandlers();

            NavigationPage.SetHasNavigationBar(this, false);
        }

        Func<Task>? INhsAppWebView.Appearing { get; set; }

        public Func<OpenWebIntegrationRequest, Task>? OpenWebIntegrationRequested { get; set; }
        public Func<StartNhsLoginUpliftRequest, Task>? StartNhsLoginUpliftRequested { get; set; }

        public Func<Task>? GetNotificationsStatusRequested { get; set; }

        public Func<Task>? ResetAndShowErrorRequested { get; set; }

        public AsyncCommand<OpenWebIntegrationRequest> OpenWebIntegrationCommand
            => new AsyncCommand<OpenWebIntegrationRequest>(() => OpenWebIntegrationRequested);
        public AsyncCommand<StartNhsLoginUpliftRequest> StartNhsLoginUpliftCommand
            => new AsyncCommand<StartNhsLoginUpliftRequest>(() => StartNhsLoginUpliftRequested);

        public AsyncCommand GetNotificationsStatusCommand
            => new AsyncCommand(() => GetNotificationsStatusRequested);

        private AsyncCommand AppearingCommand => new AsyncCommand(() => ((INhsAppWebView)this).Appearing);

        public Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }
        private AsyncCommand<WebNavigatingEventArgs> NavigatingCommand => new AsyncCommand<WebNavigatingEventArgs>(() => Navigating);

        public Func<WebNavigatedEventArgs, Task>? Navigated { get; set; }
        private AsyncCommand<WebNavigatedEventArgs> NavigatedCommand => new AsyncCommand<WebNavigatedEventArgs>(() => Navigated);

        protected override void OnAppearing()
        {
            base.OnAppearing();

            RemoveEventHandlers();
            AddEventHandlers();

            AppearingCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

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

        public void GoToUri(Uri uri) => WebView.GoToUri(uri);

        public async Task NavigateToAdvice()
            => await WebView.EvaluateJavaScriptAsync($"window.nativeAppCallbacks.navigationGoToAdvice()").PreserveThreadContext();

        public async Task NavigateToAppointments()
            => await WebView.EvaluateJavaScriptAsync($"window.nativeAppCallbacks.navigationGoToAppointments()").PreserveThreadContext();

        public async Task NavigateToPrescriptions()
            => await WebView.EvaluateJavaScriptAsync($"window.nativeAppCallbacks.navigationGoToPrescriptions()").PreserveThreadContext();

        public async Task NavigateToYourHealth()
            => await WebView.EvaluateJavaScriptAsync($"window.nativeAppCallbacks.navigationGoToYourHealth()").PreserveThreadContext();

        public async Task NavigateToMessages()
            => await WebView.EvaluateJavaScriptAsync($"window.nativeAppCallbacks.navigationGoToMessages()").PreserveThreadContext();

         public async Task NavigateToSettings()
            => await WebView.EvaluateJavaScriptAsync($"window.nativeAppCallbacks.navigationGoToSettings()").PreserveThreadContext();

         public async Task NavigateToHome()
             => await WebView.EvaluateJavaScriptAsync($"window.nativeAppCallbacks.navigationGoToHome()").PreserveThreadContext();

         public async Task NavigateToRedirectedPathWithinApp(string spaPath)
             => await WebView.EvaluateJavaScriptAsync($"window.nativeAppCallbacks.navigationGoTo({ConvertToJsonString(spaPath)})").PreserveThreadContext();

         public async Task SendNotificationsStatus(string status)
             => await WebView.EvaluateJavaScriptAsync($"window.nativeAppCallbacks.notificationsSettingsStatus({ConvertToJsonString(status)})").PreserveThreadContext();

         public async Task ResetAndShowError()
            => await (ResetAndShowErrorRequested?.Invoke() ?? Task.CompletedTask).PreserveThreadContext();

        private static string ConvertToJsonString(string arg)
        {
            return JsonConvert.SerializeObject(arg);
        }
    }
}
