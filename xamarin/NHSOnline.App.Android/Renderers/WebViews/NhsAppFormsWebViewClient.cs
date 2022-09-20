using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Android.Net.Http;
using Android.Runtime;
using Android.Webkit;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Droid.Renderers.WebViews.Extensions;
using Xamarin.Forms.Platform.Android;
using Uri = Android.Net.Uri;

namespace NHSOnline.App.Droid.Renderers.WebViews
{
    internal class NhsAppFormsWebViewClient : FormsWebViewClient
    {
        private readonly WebViewRenderer? _renderer;
        private readonly IReadOnlyCollection<WebViewRendererExtension> _extensions;

        public NhsAppFormsWebViewClient(WebViewRenderer renderer, IReadOnlyCollection<WebViewRendererExtension> extensions)
            : base(renderer)
        {
            _renderer = renderer;
            _extensions = extensions;
        }

        // Unused constructor is used by java interop and should not be removed
        public NhsAppFormsWebViewClient(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
            _renderer = null;
            _extensions = Array.Empty<WebViewRendererExtension>();
        }


        [SuppressMessage("Reliability", "CA2000", Justification = "ProxyErrorWebResourceRequest is passed through to the framework where it will be disposed")]
        public override void OnReceivedError(WebView view, IWebResourceRequest request, WebResourceError error)
            => base.OnReceivedError(view, GetResourceRequestFromErrorCode(error.ErrorCode, request), error);

        private IWebResourceRequest GetResourceRequestFromErrorCode(ClientError errorCode, IWebResourceRequest request)
            => errorCode switch
            {
                ClientError.ProxyAuthentication => new ProxyErrorWebResourceRequest(request, _renderer),
                _ => request
            };

        public override void OnReceivedSslError(WebView? view, SslErrorHandler? handler, SslError? error)
        {
            if (_renderer?.Element is WebIntegrationWebView sslErrorResult)
            {
                var sslErrorDetails = new SslErrorDetails
                {
                    Url = error?.Url,
                    Error = error?.PrimaryError.ToString()
                };

                handler?.Cancel();
                sslErrorResult.OnSslError(sslErrorDetails);
                return;
            }

            base.OnReceivedSslError(view, handler, error);
        }

        private class ProxyErrorWebResourceRequest : Java.Lang.Object, IWebResourceRequest
        {
            private readonly IWebResourceRequest _request;
            private readonly WebViewRenderer? _renderer;

            public ProxyErrorWebResourceRequest(IWebResourceRequest request, WebViewRenderer? renderer)
            {
                _request = request;
                _renderer = renderer;
            }

            public bool HasGesture => _request.HasGesture;

            public bool IsForMainFrame => _request.IsForMainFrame;

            public bool IsRedirect => _request.IsRedirect;

            public string? Method => _request.Method;

            public IDictionary<string, string>? RequestHeaders => _request.RequestHeaders;

            public Uri? Url => Uri.Parse(_renderer?.Control.Url);
        }

        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            foreach (var extension in _extensions)
            {
                NhsAppResilience.ExecuteImmediately(() => extension.ShouldOverrideUrlLoading(request));
            }

            return base.ShouldOverrideUrlLoading(view, request);
        }

        [Obsolete("This obsolete but will still be called on devices running < API 24 versions, thus we handle it in addition to the new one")]
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            foreach (var extension in _extensions)
            {
                NhsAppResilience.ExecuteImmediately(() => extension.ShouldOverrideUrlLoading(url));
            }

            return base.ShouldOverrideUrlLoading(view, url);
        }

        public override void OnPageFinished(WebView view, string url)
        {
            foreach (var extension in _extensions)
            {
                NhsAppResilience.ExecuteImmediately(() => extension.OnPageFinished(url));
            }

            base.OnPageFinished(view, url);
        }
    }
}