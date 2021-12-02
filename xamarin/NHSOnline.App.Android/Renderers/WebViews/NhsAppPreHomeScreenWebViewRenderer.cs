using Android.Content;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews.Extensions;
using NHSOnline.App.Droid.Renderers.WebViews.Extensions.Javascript;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(NhsAppPreHomeScreenWebview), typeof(NhsAppPreHomeScreenWebViewRenderer))]
namespace NHSOnline.App.Droid.Renderers.WebViews
{
    public sealed class NhsAppPreHomeScreenWebViewRenderer: BaseWebViewRenderer
    {
        public NhsAppPreHomeScreenWebViewRenderer(Context context) : base(context)
        {
            AddExtension(new AccessibilityWebViewRendererExtension(this));
            AddExtension(new PreHomeScreenJavascriptBridgeWebViewRendererExtension(this));
            AddExtension(new PageNavigationAggregatorExtension());
        }
    }
}
