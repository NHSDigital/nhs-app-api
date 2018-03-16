using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NHSOnline.Backend.Worker.Suppliers.Emis;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.Worker.UnitTests.Suppliers.Emis.Helpers
{
    public static class EmisHttpMockingExtensions
    {
        public static MockedRequest WhenEmis(this MockHttpMessageHandler handler, HttpMethod method,
            string relativePath)
        {
            var url = new Uri(EmisClientTests.BaseUri, relativePath);
            return handler.When(method, url.ToString());
        }

        public static MockedRequest WithEmisHeaders(this MockedRequest mockedRequest,
            IList<KeyValuePair<string, string>> headers = null)
        {
            headers = headers ?? new List<KeyValuePair<string, string>>();

            if (headers.All(x => x.Key != EmisClient.HeaderApplicationId))
            {
                headers.Add(new KeyValuePair<string, string>(EmisClient.HeaderApplicationId,
                    EmisClientTests.DefaultEmisApplicationId));
            }

            if (headers.All(x => x.Key != EmisClient.HeaderVersion))
            {
                headers.Add(new KeyValuePair<string, string>(EmisClient.HeaderVersion,
                    EmisClientTests.DefaultEmisVersion));
            }

            mockedRequest.WithHeaders(headers);

            return mockedRequest;
        }
    }
}