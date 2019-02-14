using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp
{
    public static class TppHttpMockingExtensions
    {
        public static MockedRequest WhenTpp(this MockHttpMessageHandler handler, HttpMethod method,
            Uri apiUrl)
        {
            return handler.When(method, apiUrl.ToString());
        }

        public static MockedRequest WithTppHeaders(this MockedRequest mockedRequest,
            IList<KeyValuePair<string, string>> headers = null)
        {
            headers = headers ?? new List<KeyValuePair<string, string>>();

            if (headers.All(x => !x.Key.Equals(TppClient.RequestTypeHeader, StringComparison.Ordinal)))
            {
                headers.Add(new KeyValuePair<string, string>(TppClient.RequestTypeHeader, TppClientTests.DefaultTypeHeaderValue));
            }

            mockedRequest.WithHeaders(headers);

            return mockedRequest;
        }       
    }
}
