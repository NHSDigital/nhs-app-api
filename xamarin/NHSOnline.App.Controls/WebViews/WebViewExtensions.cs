using System;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.WebViews
{
    public static class WebViewExtensions
    {
        public static void GoToUri(this WebView webView, Uri uri)
        {
            webView.Source = new UrlWebViewSource {Url = uri.ToString()};
        }
    }
}