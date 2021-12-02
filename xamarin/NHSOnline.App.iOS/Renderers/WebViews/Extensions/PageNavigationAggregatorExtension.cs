using System;
using System.Collections.Generic;
using NHSOnline.App.Controls.WebViews;
using WebKit;
using Xamarin.Forms.Platform.iOS;

namespace NHSOnline.App.iOS.Renderers.WebViews.Extensions
{
    internal sealed class PageNavigationAggregatorExtension : IWebViewRendererExtension
    {
        private readonly WKWebView _webView;

        private readonly List<(Uri, DateTimeOffset)> _navigationFlow = new();
        private INavigationFlowAwareWebView? _navigationFlowAwareWebView;

        public PageNavigationAggregatorExtension(WKWebView webView)
        {
            _webView = webView;
        }

        public void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.NewElement is INavigationFlowAwareWebView navigationFlowAwareWebView)
            {
                _navigationFlowAwareWebView = navigationFlowAwareWebView;
            }
            else if (e.NewElement != null)
            {
                throw new NotSupportedException(
                    $"The {nameof(PageNavigationAggregatorExtension)} extension can only be added to WebViews that implement {nameof(INavigationFlowAwareWebView)}");
            }
        }

        public void DidStartProvisionalNavigation(WKNavigation navigation)
        {
            var url = _webView.Url?.ToString();
            if (url != null)
            {
                _navigationFlow.Add((new Uri(url), DateTimeOffset.UtcNow));
            }
        }

        public void DidReceiveServerRedirectForProvisionalNavigation(WKNavigation navigation)
        {
            var url = _webView.Url?.ToString();
            if (url != null)
            {
                _navigationFlow.Add((new Uri(url), DateTimeOffset.UtcNow));
            }
        }

        public void DidFinishNavigation(WKNavigation navigation)
        {
            _navigationFlowAwareWebView?.OnPageLoadComplete(new WebViewPageNavigationEventArgs(_navigationFlow));
        }
    }
}