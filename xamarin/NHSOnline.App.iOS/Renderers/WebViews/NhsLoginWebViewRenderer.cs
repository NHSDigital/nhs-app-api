using NHSOnline.App.Controls;
using NHSOnline.App.iOS.Renderers.WebViews;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NhsLoginWebView), typeof(NhsLoginWebViewRenderer))]

namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal sealed class NhsLoginWebViewRenderer : WkWebViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.OldElement == null)
            {
                NavigationDelegate = new DelegatingWebViewNavigationDelegate(NavigationDelegate);
            }

            base.OnElementChanged(e);
        }
    }
}