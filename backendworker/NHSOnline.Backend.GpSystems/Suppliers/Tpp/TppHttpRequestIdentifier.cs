using System.Linq;
using System.Net.Http;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
{
    public class TppHttpRequestIdentifier : IHttpRequestIdentifier
    {
        public HttpRequestIdentity Identify(HttpRequestMessage request)
        {
            string identifier = null;
            if (request.Headers.Contains(Constants.TppConstants.RequestIdentifierHeader))
            {
                request.Headers.TryGetValues(Constants.TppConstants.RequestIdentifierHeader, out var values);
                identifier = values.FirstOrDefault();
            }
            
            return 
                new HttpRequestIdentity($"{Supplier.Tpp}", request)
                .SetUpStreamIdentifier(identifier);
        }
    }
}