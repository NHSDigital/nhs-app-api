using System.Linq;
using System.Net.Http;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    public class TppHttpRequestIdentifier : HttpRequestIdentifier
    {
        protected override SourceApi SourceApi => SourceApi.Tpp;
        protected override string Provider => "Tpp";

        public override HttpRequestIdentity Identify(HttpRequestMessage request)
        {
            string identifier = null;
            if (request.Headers.Contains(Constants.TppConstants.RequestIdentifierHeader))
            {
                request.Headers.TryGetValues(Constants.TppConstants.RequestIdentifierHeader, out var values);
                identifier = values.FirstOrDefault();
            }

            return
                new HttpRequestIdentity(Provider, request, SourceApi)
                    .SetUpStreamIdentifier(identifier);
        }
    }
}