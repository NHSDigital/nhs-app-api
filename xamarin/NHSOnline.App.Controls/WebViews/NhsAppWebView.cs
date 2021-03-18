using NHSOnline.App.Controls.WebViews.Payloads;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.WebViews
{
    public sealed class NhsAppWebView: WebView
    {
        public static readonly BindableProperty OpenWebIntegrationCommandProperty =
            BindableProperty.Create(nameof(OpenWebIntegrationCommand), typeof(AsyncCommand<OpenWebIntegrationRequest>), typeof(NhsAppWebView));

        public static readonly BindableProperty StartNhsLoginUpliftCommandProperty =
            BindableProperty.Create(nameof(StartNhsLoginUpliftCommand), typeof(AsyncCommand<StartNhsLoginUpliftRequest>), typeof(NhsAppWebView));

        public static readonly BindableProperty GetNotificationsStatusCommandProperty =
            BindableProperty.Create(nameof(GetNotificationsStatusCommand), typeof(AsyncCommand), typeof(NhsAppWebView));

        public static readonly BindableProperty RequestPnsTokenCommandProperty =
            BindableProperty.Create(nameof(RequestPnsTokenCommand), typeof(AsyncCommand<string>), typeof(NhsAppPreHomeScreenWebview));

        public static readonly BindableProperty FetchBiometricSpecCommandProperty =
            BindableProperty.Create(nameof(FetchBiometricSpecCommand), typeof(AsyncCommand), typeof(NhsAppWebView));

        public AsyncCommand<OpenWebIntegrationRequest> OpenWebIntegrationCommand
        {
            get => (AsyncCommand<OpenWebIntegrationRequest>) GetValue(OpenWebIntegrationCommandProperty);
            set => SetValue(OpenWebIntegrationCommandProperty, value);
        }

        public AsyncCommand<StartNhsLoginUpliftRequest> StartNhsLoginUpliftCommand
        {
            get => (AsyncCommand<StartNhsLoginUpliftRequest>)GetValue(StartNhsLoginUpliftCommandProperty);
            set => SetValue(StartNhsLoginUpliftCommandProperty, value);
        }

        public AsyncCommand GetNotificationsStatusCommand
        {
            get => (AsyncCommand) GetValue(GetNotificationsStatusCommandProperty);
            set => SetValue(GetNotificationsStatusCommandProperty, value);
        }

        public AsyncCommand<string> RequestPnsTokenCommand
        {
            get => (AsyncCommand<string>) GetValue(RequestPnsTokenCommandProperty);
            set => SetValue(RequestPnsTokenCommandProperty, value);
        }

        public AsyncCommand FetchBiometricSpecCommand
        {
            get => (AsyncCommand)GetValue(FetchBiometricSpecCommandProperty);
            set => SetValue(FetchBiometricSpecCommandProperty, value);
        }
    }
}
