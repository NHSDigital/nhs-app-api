using System.Linq;
using System.Net.Http;
using NHSOnline.Backend.Worker.Support.Http;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public class TppHttpRequestIdentifier : IHttpRequestIdentifier
    {
        public HttpRequestIdentity Identify(HttpRequestMessage request)
        {
            string identifier = null;
            if (request?.Headers != null && 
                request.Headers.Contains(Constants.TppConstants.RequestIdentifierHeader))
            {
                request.Headers.TryGetValues(Constants.TppConstants.RequestIdentifierHeader, out var values);
                identifier = values.FirstOrDefault();
            }

            var requestIdentity = new HttpRequestIdentity()
            {
                Provider = $"{Supplier.Tpp}",
                Method = request?.Method?.ToString(),
                RequestUrl = request?.RequestUri,
                Identifier = identifier
            };

            return requestIdentity;
        }
    }
}