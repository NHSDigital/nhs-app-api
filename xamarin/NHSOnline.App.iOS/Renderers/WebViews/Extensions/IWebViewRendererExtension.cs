using WebKit;
using Xamarin.Forms.Platform.iOS;

namespace NHSOnline.App.iOS.Renderers.WebViews.Extensions
{
    internal interface IWebViewRendererExtension
    {
        void OnElementChanged(VisualElementChangedEventArgs e);

        void DidStartProvisionalNavigation(WKNavigation navigation)
        {
        }

        void DidReceiveServerRedirectForProvisionalNavigation(WKNavigation navigation)
        {
        }

        void DidFinishNavigation(WKNavigation navigation)
        {
        }
    }
}