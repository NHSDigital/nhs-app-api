using NHSOnline.App.Services;
using WebKit;

namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal sealed class WebViewConfigurationBuilder
    {
        private static readonly WKProcessPool SharedProcessPool = new WKProcessPool();

#pragma warning disable CA1822 // Mark members as static
        internal WKWebViewConfiguration Build()
#pragma warning restore CA1822 // Mark members as static
        {
            return new WKWebViewConfiguration
            {
                ApplicationNameForUserAgent = $"{UserAgentService.Instance.NhsAppUserAgent}",
                SuppressesIncrementalRendering = true,
                ProcessPool = WebViewConfigurationBuilder.SharedProcessPool,
            };
        }
    }
}