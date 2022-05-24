using System.Linq;
using System.Net.Http;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareHttpRequestIdentifier : HttpRequestIdentifier
    {
        protected override SourceApi SourceApi => SourceApi.SecondaryCareAggregator;

        protected override string Provider => "SecondaryCareAggregator";

        public override HttpRequestIdentity Identify(HttpRequestMessage request)
        {
            var identity = base.Identify(request);

            if (request.Headers.TryGetValues(Constants.SecondaryCareConstants.CorrelationIdHeaderKey,
                out var correlationIds))
            {
                identity.SetCorrelationIdentifier(correlationIds.First());
            }

            return identity;
        }
    }
}