using System;
using System.Collections.Generic;
using NHSOnline.App.Controls.WebViews;
using WebKit;
using Xamarin.Forms.Platform.iOS;

namespace NHSOnline.App.iOS.Renderers.WebViews.Extensions
{
    internal sealed class PageLoadRedirectAggregatorExtension : IWebViewRendererExtension
    {
        private readonly WKWebView _webView;

        private readonly List<Uri> _singleSignOnFlow = new List<Uri>();
        private IRedirectFlowAwareWebView? _redirectFlowAwareWebView;

        public PageLoadRedirectAggregatorExtension(WKWebView webView)
        {
            _webView = webView;
        }

        public void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.NewElement is IRedirectFlowAwareWebView redirectFlowAwareWebView)
            {
                _redirectFlowAwareWebView = redirectFlowAwareWebView;
            }
            else if (e.NewElement != null)
            {
                throw new NotSupportedException(
                    $"The {nameof(PageLoadRedirectAggregatorExtension)} extension can only be added to WebViews that implement {nameof(IRedirectFlowAwareWebView)}");
            }
        }

        public void DidStartProvisionalNavigation(WKNavigation navigation)
        {
            _singleSignOnFlow.Clear();

            var url = _webView.Url?.ToString();
            if (url != null)
            {
                _singleSignOnFlow.Add(new Uri(url));
            }
        }

        public void DidReceiveServerRedirectForProvisionalNavigation(WKNavigation navigation)
        {
            var url = _webView.Url?.ToString();
            if (url != null)
            {
                _singleSignOnFlow.Add(new Uri(url));
            }
        }

        public void DidFinishNavigation(WKNavigation navigation)
        {
            _redirectFlowAwareWebView?.OnPageLoadComplete(new WebViewPageLoadEventArgs(_singleSignOnFlow));
            _singleSignOnFlow.Clear();
        }
    }
}