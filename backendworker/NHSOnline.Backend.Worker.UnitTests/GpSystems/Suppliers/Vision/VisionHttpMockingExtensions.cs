using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision
{
    public static class VisionHttpMockingExtensions
    {
        public static MockedRequest WhenVision(this MockHttpMessageHandler handler, HttpMethod method,
            Uri apiUrl)
        {
            return handler.When(method, apiUrl.ToString());
        }
        
        public static MockedRequest WithVisionHeaders(this MockedRequest mockedRequest,
            string identifierValue,
            IList<KeyValuePair<string, string>> headers = null)
        {
            headers = headers ?? new List<KeyValuePair<string, string>>();

            if (headers.All(x => !x.Key.Equals(Constants.VisionConstants.RequestIdentifierHeader, StringComparison.Ordinal)))
            {
                headers.Add(new KeyValuePair<string, string>(Constants.VisionConstants.RequestIdentifierHeader, identifierValue));
            }

            mockedRequest.WithHeaders(headers);

            return mockedRequest;
        }   
    }
}