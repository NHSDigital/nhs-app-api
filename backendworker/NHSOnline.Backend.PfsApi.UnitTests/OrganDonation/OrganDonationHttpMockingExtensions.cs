using System;
using System.Net.Http;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.PfsApi.UnitTests.OrganDonation
{
    public static class OrganDonationHttpMockingExtensions
    {
        public static MockedRequest WhenOrganDonation(this MockHttpMessageHandler handler, HttpMethod method,
            string relativePath)
        {
            var url = new Uri(OrganDonationClientTests.BaseUri, relativePath);
            return handler.When(method, url.ToString());
        }
    }
}
