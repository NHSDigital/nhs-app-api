using System;
using System.Net.Http;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.NominatedPharmacy.UnitTests
{
    public static class NominatedPharmacyHttpMockingExtensions
    {
        public static MockedRequest WhenNominatedPharmacy(this MockHttpMessageHandler handler, HttpMethod method,
            Uri apiUrl)
        {
            return handler.When(method, apiUrl.ToString());
        }
    }
}