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
            if (request.Headers.Contains(Support.Constants.OnlineConsultationConstants.RequestIdentifierHeader))
            {
                request.Headers.TryGetValues(Support.Constants.OnlineConsultationConstants.RequestIdentifierHeader, out var values);
                provider = values.FirstOrDefault();
            }

            return
                new HttpRequestIdentity(provider, request, SourceApi);
        }
    }
}
