using Android.Webkit;
using Xamarin.Forms.Platform.Android;

namespace NHSOnline.App.Droid.Renderers.WebViews.Extensions
{
    internal abstract class WebViewRendererExtension
    {
        internal virtual void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        {
        }

        internal virtual void OnPageFinished(string url)
        {
        }

        internal virtual void ShouldOverrideUrlLoading(IWebResourceRequest request)
        {
        }

        internal virtual void ShouldOverrideUrlLoading(string request)
        {
        }

        internal virtual ShowFileChooserExtensionDecision OnShowFileChooser(
            WebView webView,
            IValueCallback filePathCallback,
            WebChromeClient.FileChooserParams fileChooserParams) => ShowFileChooserExtensionDecision.NotHandled;
    }
}