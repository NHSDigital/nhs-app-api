using System.Collections.Generic;
using Android.Content;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using WebView = Xamarin.Forms.WebView;

[assembly: ExportRenderer(typeof(NhsLoginWebView), typeof(NhsLoginWebViewRenderer))]
namespace NHSOnline.App.Droid.Renderers.WebViews
{
    public sealed class NhsLoginWebViewRenderer: WebViewRenderer
    {
        private readonly List<IWebViewRendererExtension> _extensions;

        public NhsLoginWebViewRenderer(Context context) : base(context)
        {
            _extensions = new List<IWebViewRendererExtension>
            {
                new UserAgentWebViewRendererExtension(this)
            };
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            foreach (var extension in _extensions)
            {
                extension.OnElementChanged(e);
            }
        }
    }
}
