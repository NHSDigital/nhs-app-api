using System;
using System.Net;
using System.Threading.Tasks;
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

        public Func<Cookie, Task>? SetCookie { get; set; }

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
    }
}
