using System.Diagnostics.CodeAnalysis;
using Android.Content;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews.Extensions;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(NhsLoginUpliftWebView), typeof(NhsLoginUpliftWebViewRenderer))]
namespace NHSOnline.App.Droid.Renderers.WebViews
{
    public sealed class NhsLoginUpliftWebViewRenderer: BaseWebViewRenderer
    {
        public NhsLoginUpliftWebViewRenderer(Context context) : base(context)
        {
            SetupExtensions();
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "The base renderer will handle the disposing of extensions")]
        private void SetupExtensions()
        {
            AddExtension(new MediaFileUploadExtension());
            AddExtension(new IProovExtension(this));
        }
    }
}
