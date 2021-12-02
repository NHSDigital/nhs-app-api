using System;
using System.Collections.Generic;
using Android.Webkit;
using NHSOnline.App.Controls.WebViews;
using Xamarin.Forms.Platform.Android;
using WebView = Xamarin.Forms.WebView;

namespace NHSOnline.App.Droid.Renderers.WebViews.Extensions
{
    internal sealed class PageLoadRedirectAggregatorExtension : WebViewRendererExtension
    {
        private IRedirectFlowAwareWebView? _redirectFlowAwareWebView;
        private readonly List<(Uri, DateTimeOffset)> _redirectFlow = new();

        internal override void OnElementChanged(ElementChangedEventArgs<WebView> e)
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

        internal override void ShouldOverrideUrlLoading(IWebResourceRequest request)
        {
            if (request.IsForMainFrame && request.Url != null)
            {
                var url = request.Url.ToString();
                if (url != null)
                {
                    _redirectFlow.Add((new Uri(url), DateTimeOffset.UtcNow));
                }
            }
        }

        internal override void ShouldOverrideUrlLoading(string url)
        {
            _redirectFlow.Add((new Uri(url), DateTimeOffset.UtcNow));
        }

        internal override void OnPageFinished(string url)
        {
            _redirectFlowAwareWebView?.OnPageLoadComplete(new WebViewPageLoadEventArgs(_redirectFlow));
            _redirectFlow.Clear();
        }
    }
}