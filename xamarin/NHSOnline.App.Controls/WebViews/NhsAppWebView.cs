using System;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.WebViews
{
    public sealed class NhsAppWebView: WebView
    {
        public Func<Cookie, Task>? SetCookie { get; set; }
    }
}
