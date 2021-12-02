using System;
using System.Collections.Generic;
using Android.Webkit;
using NHSOnline.App.Controls.WebViews;
using Xamarin.Forms.Platform.Android;
using WebView = Xamarin.Forms.WebView;

namespace NHSOnline.App.Droid.Renderers.WebViews.Extensions
{
    internal sealed class PageNavigationAggregatorExtension : WebViewRendererExtension
    {
        private INavigationFlowAwareWebView? _navigationFlowAwareWebView;
        private readonly List<(Uri, DateTimeOffset)> _navigationFlow = new();

        private string _lastUrl = string.Empty;

        internal override void OnElementChanged(ElementChangedEventArgs<WebView> e)
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

        internal override void ShouldOverrideUrlLoading(IWebResourceRequest request)
        {
            if (request.IsForMainFrame)
            {
                var url = request.Url?.ToString();
                if (url != null)
                {
                    AddUrlToNavigationFlow(url);
                }
            }
        }

        internal override void ShouldOverrideUrlLoading(string url) => AddUrlToNavigationFlow(url);

        internal override void OnPageFinished(string url)
        {
            if (!string.Equals(url, _lastUrl, StringComparison.Ordinal))
            {
                AddUrlToNavigationFlow(url);
            }

            _navigationFlowAwareWebView?.OnPageLoadComplete(new WebViewPageNavigationEventArgs(_navigationFlow));
        }

        private void AddUrlToNavigationFlow(string url)
        {
            _lastUrl = url;
            _navigationFlow.Add((new Uri(url), DateTimeOffset.UtcNow));
        }
    }
}