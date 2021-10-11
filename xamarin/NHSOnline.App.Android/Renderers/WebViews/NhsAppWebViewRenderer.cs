using Android.Content;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews.Extensions;
using NHSOnline.App.Droid.Renderers.WebViews.Extensions.Javascript;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(NhsAppWebView), typeof(NhsAppWebViewRenderer))]
namespace NHSOnline.App.Droid.Renderers.WebViews
{
    public sealed class NhsAppWebViewRenderer: BaseWebViewRenderer
    {
        public NhsAppWebViewRenderer(Context context) : base(context)
        {
            AddExtension(new AccessibilityWebViewRendererExtension(this));
            AddExtension(new NhsAppJavascriptBridgeWebViewRendererExtension(this));
        }
    }
}
