using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using Foundation;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Controls.WebViews.Payloads;
using Xamarin.Forms.Platform.iOS;

namespace NHSOnline.App.iOS.Renderers.WebViews.Extensions
{
    [SuppressMessage("Reliability", "CA2000", Justification = "Disposing is hard and causes crashes if we do it wrong")]
    internal sealed class WebIntegrationRequestRendererExtension : IWebViewRendererExtension
    {
        private readonly WkWebViewRenderer _renderer;

        public WebIntegrationRequestRendererExtension(WkWebViewRenderer renderer)
        {
            _renderer = renderer;
        }
        public void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.OldElement == null)
            {
                if (e.NewElement is WebIntegrationWebView webView)
                {
                    webView.PropertyChanged += WebViewOnPropertyChanged;
                }
            }

            LoadWebRequest();
        }

        private void LoadWebRequest()
        {
            if (_renderer.Element is WebIntegrationWebView webView)
            {
                if (webView.WebIntegrationRequest == null)
                {
                    return;
                }

                var loadRequest = CreateRequest(webView.WebIntegrationRequest);
                _renderer.LoadRequest(loadRequest);
            }
        }

        private void WebViewOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WebIntegrationWebView.WebIntegrationRequest))
            {
                LoadWebRequest();
            }
        }

        private static NSUrlRequest CreateRequest(WebIntegrationRequest request)
        {
            var nsUrlRequest = new NSMutableUrlRequest(request.Url);
            nsUrlRequest.HttpMethod = request.Verb.Method;

            if (request.Verb == HttpMethod.Post)
            {
                var headerDictionary = new NSMutableDictionary();
                headerDictionary.Add(new NSString("Content-Type"), new NSString("application/x-www-form-urlencoded"));
                nsUrlRequest.Headers = headerDictionary;

                var formData = request.PostData?.ToFormUrlEncodedString() ?? string.Empty;
                nsUrlRequest.Body = NSData.FromString(formData);
            }

            return nsUrlRequest;
        }
    }
}