using System;
using System.Net.Http;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.PfsApi.UnitTests.Extensions
{
    public static class HttpMockingExtensions
    {
        public static MockedRequest WhenRequest(
            this MockHttpMessageHandler handler,
            HttpMethod method,
            Uri baseUri,
            string relativePath
        )
        {
            var url = new Uri(baseUri, relativePath);
            return handler.When(method, url.ToString());
        }
    }
}
