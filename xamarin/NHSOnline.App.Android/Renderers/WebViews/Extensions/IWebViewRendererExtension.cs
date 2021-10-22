using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace NHSOnline.App.Droid.Renderers.WebViews.Extensions
{
    public interface IWebViewRendererExtension
    {
        void OnElementChanged(ElementChangedEventArgs<WebView> e);
    }
}