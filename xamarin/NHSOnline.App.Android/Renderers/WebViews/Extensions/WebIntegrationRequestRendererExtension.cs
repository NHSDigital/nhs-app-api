using System;
using System.ComponentModel;
using System.Net.Http;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Controls.WebViews.Payloads;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace NHSOnline.App.Droid.Renderers.WebViews.Extensions
{
    internal sealed class WebIntegrationRequestRendererExtension: IWebViewRendererExtension
    {
        private readonly WebViewRenderer _renderer;

        public WebIntegrationRequestRendererExtension(WebViewRenderer renderer)
        {
            _renderer = renderer;
        }

        public void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            if (e.OldElement == null)
            {

                if (e.NewElement is WebIntegrationWebView webView)
                {
                    webView.PropertyChanged += NewWebIntegrationWebViewOnPropertyChanged;
                }
            }

            LoadWebRequest();
        }

        private void NewWebIntegrationWebViewOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WebIntegrationWebView.WebIntegrationRequest))
            {
                LoadWebRequest();
            }
        }

        private void LoadWebRequest()
        {
            if (_renderer.Element is WebIntegrationWebView webView)
            {
                var webRequest = webView.WebIntegrationRequest;
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