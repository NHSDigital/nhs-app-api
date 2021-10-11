using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Webkit;
using NHSOnline.App.Droid.Renderers.WebViews.Extensions;
using Xamarin.Forms.Platform.Android;
using WebView = Xamarin.Forms.WebView;

namespace NHSOnline.App.Droid.Renderers.WebViews
{
    public class BaseWebViewRenderer : WebViewRenderer
    {
        private readonly List<WebViewRendererExtension> _extensions = new();

        public BaseWebViewRenderer(Context context) : base(context)
        {
            AddExtension(new UserAgentWebViewRendererExtension(this));
            AddExtension(new EnableTargetBlankLinksRendererExtension(this));
        }

        private protected void AddExtension(WebViewRendererExtension extension)
        {
            _extensions.Add(extension);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            foreach (var extension in _extensions)
            {
                extension.OnElementChanged(e);
            }
        }

        protected override WebViewClient GetWebViewClient() => new NhsAppFormsWebViewClient(this, _extensions.AsReadOnly());

        protected override FormsWebChromeClient GetFormsWebChromeClient() => new BaseWebChromeClient(_extensions.AsReadOnly());

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var disposableExtension in _extensions.OfType<IDisposable>())
                {
                    disposableExtension.Dispose();
                }

                Control.Destroy();
            }

            base.Dispose(disposing);
        }
    }
}