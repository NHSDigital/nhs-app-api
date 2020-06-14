using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis
{
    public static class EmisHttpMockingExtensions
    {
        private const string HeaderApplicationId = "X-API-ApplicationId";
        private const string HeaderVersion = "X-API-Version";

        public static MockedRequest WhenEmis(
            this MockHttpMessageHandler handler,
            HttpMethod method,
            string relativePath)
        {
            var url = new Uri(EmisClientTestsContext.BaseUri, relativePath);
            return handler.When(method, url.ToString());
        }

        public static MockedRequest WithEmisHeaders(
            this MockedRequest mockedRequest,
            IList<KeyValuePair<string, string>> headers = null)
        {
            headers ??= new List<KeyValuePair<string, string>>();

            if (headers.All(x => !x.Key.Equals(HeaderApplicationId, StringComparison.Ordinal)))
            {
                headers.Add(new KeyValuePair<string, string>(HeaderApplicationId, EmisClientTestsContext.DefaultEmisApplicationId));
            }

            if (headers.All(x => !x.Key.Equals(HeaderVersion, StringComparison.Ordinal)))
            {
                headers.Add(new KeyValuePair<string, string>(HeaderVersion, EmisClientTestsContext.DefaultEmisVersion));
            }

            mockedRequest.WithHeaders(headers);

            return mockedRequest;
        }
    }
}
