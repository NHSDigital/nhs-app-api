using System;
using System.Net.Http;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest
{
    public static class MicrotestHttpMockingExtensions
    {
        public static MockedRequest WhenMicrotest(this MockHttpMessageHandler handler, HttpMethod method,
            string relativePath)
        {
            var url = new Uri(MicrotestClientTests.BaseUri, relativePath);
            return handler.When(method, url.ToString());
        }
    }
}
