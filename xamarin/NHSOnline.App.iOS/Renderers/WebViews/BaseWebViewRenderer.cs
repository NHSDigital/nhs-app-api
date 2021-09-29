using System.Collections.Generic;
using NHSOnline.App.iOS.Renderers.WebViews.Extensions;
using WebKit;
using Xamarin.Forms.Platform.iOS;

namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal class BaseWebViewRenderer: WkWebViewRenderer
    {
        private List<IWebViewRendererExtension> Extensions { get; }

        public BaseWebViewRenderer() : this(CustomConfiguration)
        { }

        internal BaseWebViewRenderer(WKWebViewConfiguration config) : base(config)
        {
            Extensions = new List<IWebViewRendererExtension>
            {
                new NavigationDelegateRendererExtension(this),
                new UIDelegateRendererExtension(this)
            };
        }

        public override bool AllowsLinkPreview => false;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            foreach (var extension in Extensions)
            {
                extension.OnElementChanged(e);
            }

            base.OnElementChanged(e);
        }

        internal void AddExtension(IWebViewRendererExtension extension)
        {
            Extensions.Add(extension);
        }

        private static WKWebViewConfiguration CustomConfiguration
            => new WebViewConfigurationBuilder().Build();
    }
}