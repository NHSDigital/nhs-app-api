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

        public Func<Cookie, Task>? SetCookie { get; set; }

        public AsyncCommand<OpenWebIntegrationRequest> OpenWebIntegrationCommand
        {
            get => (AsyncCommand<OpenWebIntegrationRequest>) GetValue(OpenWebIntegrationCommandProperty);
            set => SetValue(OpenWebIntegrationCommandProperty, value);
        }
    }
}
