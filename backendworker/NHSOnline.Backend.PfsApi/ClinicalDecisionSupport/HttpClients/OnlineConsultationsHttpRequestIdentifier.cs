using System.Linq;
using System.Net.Http;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients
{
    public class OnlineConsultationsHttpRequestIdentifier : HttpRequestIdentifier
    {
        protected override SourceApi SourceApi => SourceApi.OnlineConsultations;
        protected override string Provider => "OLC";

        public override HttpRequestIdentity Identify(HttpRequestMessage request)
        {
            string provider = null;
            if (request.Headers.Contains(Support.Constants.OnlineConsultationConstants.ProviderIdentifierHeader))
            {
                request.Headers.TryGetValues(Support.Constants.OnlineConsultationConstants.ProviderIdentifierHeader, out var values);
                provider = values.FirstOrDefault();
            }
            
            string olcSessionId = null;
            if (request.Headers.Contains(Support.Constants.OnlineConsultationConstants.SessionIdentifierHeader))
            {
                request.Headers.TryGetValues(Support.Constants.OnlineConsultationConstants.SessionIdentifierHeader, out var values);
                olcSessionId = values.FirstOrDefault();
            }

            return olcSessionId == null
                ? new HttpRequestIdentity(provider, request, SourceApi)
                : new HttpRequestIdentity(provider, olcSessionId, request, SourceApi);
        }
    }
}
