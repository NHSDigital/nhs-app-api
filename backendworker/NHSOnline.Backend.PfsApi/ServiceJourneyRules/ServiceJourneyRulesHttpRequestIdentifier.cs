using System.Net.Http;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.ServiceJourneyRules
{
    public class ServiceJourneyRulesHttpRequestIdentifier : IHttpRequestIdentifier
    {
        public HttpRequestIdentity Identify(HttpRequestMessage request)
        {
            return new HttpRequestIdentity("ServiceJourneyRules", request);
        }
    }
}