using System;
using System.Net;
using System.Threading.Tasks;
using NHSOnline.App.Controls.WebViews.KnownServices;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.WebViews
{
    public sealed class NhsAppWebView: WebView
    {
        public static readonly BindableProperty OpenWebIntegrationCommandProperty =
            BindableProperty.Create(nameof(OpenWebIntegrationCommand), typeof(Command<OpenWebIntegrationRequest>), typeof(NhsAppWebView));

        public Func<Cookie, Task>? SetCookie { get; set; }

        public Command<OpenWebIntegrationRequest> OpenWebIntegrationCommand
        {
            get => (Command<OpenWebIntegrationRequest>) GetValue(OpenWebIntegrationCommandProperty);
            set => SetValue(OpenWebIntegrationCommandProperty, value);
        }
    }
}
