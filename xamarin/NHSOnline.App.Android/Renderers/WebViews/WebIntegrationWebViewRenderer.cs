using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

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
                new WebIntegrationJavascriptBridgeWebViewRendererExtension(this)
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
