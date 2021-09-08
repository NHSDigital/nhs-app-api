using System;
using System.Collections.Generic;
using System.Net.Http;

namespace NHSOnline.App.Controls.WebViews.Payloads
{
    public sealed class WebIntegrationRequest
    {
        private WebIntegrationRequest(HttpMethod verb, Uri url, IReadOnlyDictionary<string, string>? postData)
        {
            Verb = verb;
            Url = url;
            PostData = postData;
        }

        public HttpMethod Verb { get; }
        public Uri Url { get; }
        public IReadOnlyDictionary<string, string>? PostData { get; }

        public static WebIntegrationRequest Create(OpenPostWebIntegrationRequest request)
        {
            return new WebIntegrationRequest(
                HttpMethod.Post,
                request.Url,
                request.PostData);
        }

        public static WebIntegrationRequest Create(OpenWebIntegrationRequest request)
        {
            return new WebIntegrationRequest(
                HttpMethod.Get,
                request.Url,
                null);
        }
    }

}