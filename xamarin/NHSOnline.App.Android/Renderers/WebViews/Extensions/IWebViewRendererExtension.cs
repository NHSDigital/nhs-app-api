using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace NHSOnline.App.Droid.Renderers.WebViews.Extensions
{
    internal interface IWebViewRendererExtension
    {
        void OnElementChanged(ElementChangedEventArgs<WebView> e);
    }
}