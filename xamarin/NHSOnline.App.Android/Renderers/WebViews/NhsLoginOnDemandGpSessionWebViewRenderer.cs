using Android.Content;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(NhsLoginOnDemandGpSessionWebView), typeof(NhsLoginOnDemandGpSessionWebViewRenderer))]
namespace NHSOnline.App.Droid.Renderers.WebViews
{
    public sealed class NhsLoginOnDemandGpSessionWebViewRenderer: BaseWebViewRenderer
    {
        public NhsLoginOnDemandGpSessionWebViewRenderer(Context context) : base(context)
        {
        }
    }
}
