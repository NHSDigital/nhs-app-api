using System;
using System.Collections.Generic;
using System.Net.Http;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.Support;
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

        public static MockedRequest WithMicrotestHeaders(this MockedRequest mockedRequest, string odsCode, string nhsNumber)
        {
            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(MicrotestClient.HeaderNhsNumber, nhsNumber.RemoveWhiteSpace()),
                new KeyValuePair<string, string>(MicrotestClient.HeaderOdsCode, odsCode)
            };

            mockedRequest.WithHeaders(headers);

            return mockedRequest;
        }
    }
}
