using Android.Content;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews.Extensions;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(NhsLoginWebView), typeof(NhsLoginWebViewRenderer))]
namespace NHSOnline.App.Droid.Renderers.WebViews
{
    public sealed class NhsLoginWebViewRenderer: BaseWebViewRenderer
    {
        public NhsLoginWebViewRenderer(Context context) : base(context)
        {
            AddExtension(new UserAgentWebViewRendererExtension(this));
            AddExtension(new EnableTargetBlankLinksRendererExtension(this));
        }
    }
}
