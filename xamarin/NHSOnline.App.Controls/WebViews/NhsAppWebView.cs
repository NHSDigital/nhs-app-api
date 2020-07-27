using System;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.WebViews
{
    public sealed class NhsAppWebView: WebView
    {
        public static readonly BindableProperty NavigateToThirdPartyCommandProperty =
            BindableProperty.Create(nameof(NavigateToThirdPartyCommand), typeof(Command<string>), typeof(NhsAppWebView));

        public Func<Cookie, Task>? SetCookie { get; set; }

        public Command<string> NavigateToThirdPartyCommand
        {
            get => (Command<string>) GetValue(NavigateToThirdPartyCommandProperty);
            set => SetValue(NavigateToThirdPartyCommandProperty, value);
        }
    }
}
