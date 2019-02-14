using System.Net.Http;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    public class OrganDonationHttpRequestIdentifier : IHttpRequestIdentifier
    {
        public HttpRequestIdentity Identify(HttpRequestMessage request)
        {
            var requestIdentity = new HttpRequestIdentity()
            {
                Provider = "OrganDonation",
                Method = request?.Method?.ToString(),
                RequestUrl = request?.RequestUri,
                Identifier = null
            };

            return requestIdentity;
        }
    }
}