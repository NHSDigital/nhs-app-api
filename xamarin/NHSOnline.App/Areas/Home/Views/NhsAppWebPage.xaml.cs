using System;
using System.ComponentModel;
using System.Threading.Tasks;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Threading;
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

        public Func<string, Task>? GetPnsTokenRequested { get; set; }

        public Func<Task>? ResetAndShowErrorRequested { get; set; }

        public AsyncCommand<OpenWebIntegrationRequest> OpenWebIntegrationCommand
            => new AsyncCommand<OpenWebIntegrationRequest>(() => OpenWebIntegrationRequested);

        public AsyncCommand<StartNhsLoginUpliftRequest> StartNhsLoginUpliftCommand
            => new AsyncCommand<StartNhsLoginUpliftRequest>(() => StartNhsLoginUpliftRequested);

        public AsyncCommand GetNotificationsStatusCommand => new AsyncCommand(() => GetNotificationsStatusRequested);

        public AsyncCommand<string> RequestPnsTokenCommand => new AsyncCommand<string>(() => GetPnsTokenRequested);

        public Func<Task>? FetchBiometricSpecRequested { get; set; }
        public AsyncCommand FetchBiometricSpecCommand => new AsyncCommand(() => FetchBiometricSpecRequested);

        public Func<string, Task>? UpdateBiometricRegistrationRequested { get; set; }
        public AsyncCommand<string> UpdateBiometricRegistrationCommand => new AsyncCommand<string>(() => UpdateBiometricRegistrationRequested);

        private AsyncCommand AppearingCommand => new AsyncCommand(() => ((INhsAppWebView) this).Appearing);

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

        public async Task NavigateToAdvice() => await WebView.NavigateToAdvice().ResumeOnThreadPool();

        public async Task NavigateToAppointments() => await WebView.NavigateToAppointments().ResumeOnThreadPool();

        public async Task NavigateToPrescriptions() => await WebView.NavigateToPrescriptions().ResumeOnThreadPool();

        public async Task NavigateToYourHealth() => await WebView.NavigateToYourHealth().ResumeOnThreadPool();

        public async Task NavigateToMessages() => await WebView.NavigateToMessages().ResumeOnThreadPool();

        public async Task NavigateToSettings() => await WebView.NavigateToSettings().ResumeOnThreadPool();

        public async Task NavigateToHome() => await WebView.NavigateToHome().ResumeOnThreadPool();

        public async Task NavigateToRedirectedPathWithinApp(string spaPath)
            => await WebView.NavigateToRedirectedPathWithinApp(spaPath).ResumeOnThreadPool();

        public async Task SendNotificationsStatus(string status)
            => await WebView.SendNotificationsStatus(status).ResumeOnThreadPool();

        public async Task SendBiometricSpec(BiometricSpec biometricSpec)
            => await WebView.SendBiometricSpec(biometricSpec).ResumeOnThreadPool();

        public async Task SendBiometricCompletion(BiometricCompletion completionDetails)
            => await WebView.SendBiometricCompletion(completionDetails).ResumeOnThreadPool();

        public async Task ResetAndShowError()
            => await (ResetAndShowErrorRequested?.Invoke() ?? Task.CompletedTask).ResumeOnThreadPool();

        public async Task SendNotificationAuthorised(NotificationAuthorisedResponse authorisedResponse)
            => await WebView.SendNotificationAuthorised(authorisedResponse).ResumeOnThreadPool();

        public async Task SendNotificationUnauthorised() => await WebView.SendNotificationUnauthorised().ResumeOnThreadPool();
        
        private void WebViewNavigating(object sender, WebNavigatingEventArgs e)
        {
            Spinner.IsVisible = true;
            WebView.IsVisible = false;

        }

        private void WebOnEndNavigating(object sender, WebNavigatedEventArgs e)
        {
            Spinner.IsVisible = false;
            WebView.IsVisible = true;
        }
    }
}
