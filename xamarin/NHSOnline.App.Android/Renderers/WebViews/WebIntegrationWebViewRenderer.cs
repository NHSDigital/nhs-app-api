using Android.Content;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews.Extensions;
using NHSOnline.App.Droid.Renderers.WebViews.Extensions.Javascript;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(WebIntegrationWebView), typeof(WebIntegrationWebViewRenderer))]
namespace NHSOnline.App.Droid.Renderers.WebViews
{
    public sealed class WebIntegrationWebViewRenderer : BaseWebViewRenderer
    {
        public WebIntegrationWebViewRenderer(Context context) : base(context)
        {
            AddExtension(new WebIntegrationJavascriptBridgeWebViewRendererExtension(this));
            AddExtension(new WebIntegrationRequestRendererExtension(this));
            AddExtension(new PageLoadRedirectAggregatorExtension());
        }
    }
}
