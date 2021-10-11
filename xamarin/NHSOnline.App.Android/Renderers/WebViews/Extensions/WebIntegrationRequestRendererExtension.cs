using System;
using System.ComponentModel;
using System.Net.Http;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Controls.WebViews.Payloads;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace NHSOnline.App.Droid.Renderers.WebViews.Extensions
{
    internal sealed class WebIntegrationRequestRendererExtension: WebViewRendererExtension
    {
        private readonly WebViewRenderer _renderer;
        private IPostRequestCapableWebView? _postRequestCapableWebView;

        public WebIntegrationRequestRendererExtension(WebViewRenderer renderer)
        {
            _renderer = renderer;
        }

        internal override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            if (e.NewElement is IPostRequestCapableWebView postRequestCapableWebView)
            {
                _postRequestCapableWebView = postRequestCapableWebView;
                e.NewElement.PropertyChanged += WebViewOnPropertyChanged;
            }
            else if (e.NewElement != null)
            {
                throw new NotSupportedException(
                    $"The {nameof(WebIntegrationRequestRendererExtension)} extension can only be added to WebViews that implement {nameof(IPostRequestCapableWebView)}");
            }

            LoadWebRequest();
        }

        private void WebViewOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IPostRequestCapableWebView.WebIntegrationRequest))
            {
                LoadWebRequest();
            }
        }

        private void LoadWebRequest()
        {
            var webRequest = _postRequestCapableWebView?.WebIntegrationRequest;
            if (webRequest != null)
            {
                if (webRequest.Verb == HttpMethod.Post)
                {
                    PostRequest(webRequest);
                }
                else
                {
                    GetRequest(webRequest);
                }
            }
        }

        private void PostRequest(WebIntegrationRequest request)
        {
            var postData = request.PostData?.ToFormUrlEncodedBytes() ?? Array.Empty<byte>();
            _renderer.Control.PostUrl(request.Url.AbsoluteUri, postData);
        }

        private void GetRequest(WebIntegrationRequest request)
        {
            _renderer.Control.LoadUrl(request.Url.AbsoluteUri);
        }
    }
}