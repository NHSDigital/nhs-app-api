using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;
using RichardSzalay.MockHttp;
using RichardSzalay.MockHttp.Matchers;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp
{
    public static class TppHttpMockingExtensions
    {
        public static MockedRequest WhenTpp(this MockHttpMessageHandler handler, HttpMethod method,
            string apiUrl)
        {
            return handler.When(method, apiUrl);
        }

        public static MockedRequest WithTppHeaders(this MockedRequest mockedRequest,
            IList<KeyValuePair<string, string>> headers = null)
        {
            headers = headers ?? new List<KeyValuePair<string, string>>();

            if (headers.All(x => x.Key != TppClient.RequestTypeHeader))
            {
                headers.Add(new KeyValuePair<string, string>(TppClient.RequestTypeHeader, TppClientTests.DefaultTypeHeaderValue));
            }

            mockedRequest.WithHeaders(headers);

            return mockedRequest;
        }       
    }
}