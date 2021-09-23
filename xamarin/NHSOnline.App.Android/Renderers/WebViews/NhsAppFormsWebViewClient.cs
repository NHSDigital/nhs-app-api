using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Android.Runtime;
using Android.Webkit;
using Xamarin.Forms.Platform.Android;
using Uri = Android.Net.Uri;

namespace NHSOnline.App.Droid.Renderers.WebViews
{
    public class NhsAppFormsWebViewClient : FormsWebViewClient
    {
        private readonly WebViewRenderer? _renderer;

        public NhsAppFormsWebViewClient(WebViewRenderer renderer) : base(renderer)
            => _renderer = renderer;

        // Unused constructor is used by java interop and should not be removed
        public NhsAppFormsWebViewClient(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
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
    }
}