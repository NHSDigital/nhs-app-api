using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Webkit;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews.Extensions;
using NHSOnline.App.Droid.Renderers.WebViews.Extensions.Javascript;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using WebView = Xamarin.Forms.WebView;

[assembly: ExportRenderer(typeof(WebIntegrationWebView), typeof(WebIntegrationWebViewRenderer))]
namespace NHSOnline.App.Droid.Renderers.WebViews
{
    public sealed class WebIntegrationWebViewRenderer : WebViewRenderer
    {
        private readonly List<IWebViewRendererExtension> _extensions;

        public WebIntegrationWebViewRenderer(Context context) : base(context)
        {
            _extensions = new List<IWebViewRendererExtension>
            {
                new UserAgentWebViewRendererExtension(this),
                new WebIntegrationJavascriptBridgeWebViewRendererExtension(this),
                new EnableTargetBlankLinksRendererExtension(this),
                new WebIntegrationRequestRendererExtension(this)
            };
        }

        protected override WebViewClient GetWebViewClient() => new NhsAppFormsWebViewClient(this);

        protected override FormsWebChromeClient GetFormsWebChromeClient() => new BaseChromeClient();

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            foreach (var extension in _extensions)
            {
                extension.OnElementChanged(e);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var disposableExtension in _extensions.OfType<IDisposable>())
                {
                    disposableExtension.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
