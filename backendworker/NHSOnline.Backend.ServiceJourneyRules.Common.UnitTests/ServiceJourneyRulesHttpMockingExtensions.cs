using System;
using System.Net.Http;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.ServiceJourneyRules.Common.UnitTests
{
    public static class ServiceJourneyRulesHttpMockingExtensions
    {
        public static MockedRequest WhenServiceJourneyRules(this MockHttpMessageHandler handler, HttpMethod method,
            string relativePath)
        {
            var url = new Uri(ServiceJourneyRulesClientTests.BaseUri, relativePath);
            return handler.When(method, url.ToString());
        }
    }
}
