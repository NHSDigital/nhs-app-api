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
    public abstract class BaseWebViewRenderer: WebViewRenderer
    {
        private readonly List<IWebViewRendererExtension> _extensions;

        protected BaseWebViewRenderer(Context context) : base(context)
        {
            _extensions = new List<IWebViewRendererExtension>();
        }

        protected void AddExtension(IWebViewRendererExtension extension)
        {
            _extensions.Add(extension);
        }

        protected override WebViewClient GetWebViewClient() => new NhsAppFormsWebViewClient(this);

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

                Control.Destroy();
            }
            base.Dispose(disposing);
        }
    }
}