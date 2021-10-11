using System.Collections.Generic;
using NHSOnline.App.iOS.Renderers.WebViews.Extensions;
using WebKit;
using Xamarin.Forms.Platform.iOS;

namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal class BaseWebViewRenderer: WkWebViewRenderer
    {
        private readonly List<IWebViewRendererExtension> _extensions = new List<IWebViewRendererExtension>();

        public BaseWebViewRenderer() : this(CustomConfiguration)
        { }

        internal BaseWebViewRenderer(WKWebViewConfiguration config) : base(config)
        {
        }

        public override bool AllowsLinkPreview => false;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.OldElement != null)
            {
                NavigationDelegate = new WebViewNavigationDelegate(this, _extensions.AsReadOnly());
                UIDelegate = new WebViewUIDelegate();
            }

            foreach (var extension in _extensions)
            {
                extension.OnElementChanged(e);
            }

            base.OnElementChanged(e);
        }

        internal void AddExtension(IWebViewRendererExtension extension)
        {
            _extensions.Add(extension);
        }

        private static WKWebViewConfiguration CustomConfiguration
            => new WebViewConfigurationBuilder().Build();
    }
}