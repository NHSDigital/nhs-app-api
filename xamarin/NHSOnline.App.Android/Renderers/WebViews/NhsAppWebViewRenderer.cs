using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews;
using NHSOnline.App.Droid.Renderers.WebViews.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using WebView = Xamarin.Forms.WebView;

[assembly: ExportRenderer(typeof(NhsAppWebView), typeof(NhsAppWebViewRenderer))]
namespace NHSOnline.App.Droid.Renderers.WebViews
{
    public sealed class NhsAppWebViewRenderer: WebViewRenderer
    {
        private readonly List<IWebViewRendererExtension> _extensions;

        public NhsAppWebViewRenderer(Context context) : base(context)
        {
            _extensions = new List<IWebViewRendererExtension>
            {
                new UserAgentWebViewRendererExtension(this),
                new NhsAppSetCookieWebViewRendererExtension(),
                new NhsAppJavascriptBridgeWebViewRendererExtension(this),
                new EnableTargetBlankLinksRendererExtension(this)
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
