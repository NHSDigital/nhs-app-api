using System.Linq;
using System.Net.Http;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    public class OrganDonationHttpRequestIdentifier : IHttpRequestIdentifier
    {
        public HttpRequestIdentity Identify(HttpRequestMessage request)
        {
            var requestIdentity =
                new HttpRequestIdentity("OrganDonation", request);

            if (request.Method == HttpMethod.Get)
            {
                return requestIdentity;
            }

            var correlationIdentifier = new OrganDonationCorrelationIdentifier();
            
            if (request.Headers.Contains(Constants.OrganDonationConstants.SessionIdHeaderKey))
            {
                request.Headers.TryGetValues(Constants.OrganDonationConstants.SessionIdHeaderKey, out var values);
                correlationIdentifier.SessionIdHeaderKey = values.FirstOrDefault();
            }

            if (request.Headers.Contains(Constants.OrganDonationConstants.SequenceIdHeaderKey))
            {
                request.Headers.TryGetValues(Constants.OrganDonationConstants.SequenceIdHeaderKey, out var values);
                correlationIdentifier.SequenceIdHeaderKey = values.FirstOrDefault();
            }

            return requestIdentity.SetCorrelationIdentifier(correlationIdentifier.SerializeJson());
        }

        private class OrganDonationCorrelationIdentifier
        {
            public string SessionIdHeaderKey { get; set; }
            public string SequenceIdHeaderKey { get; set; }
        }
    }
}