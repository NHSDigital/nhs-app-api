using System.Net.Http;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest
{
    public class MicrotestHttpRequestIdentifier : IHttpRequestIdentifier
    {
        public HttpRequestIdentity Identify(HttpRequestMessage request)
        {
            var requestIdentity = new HttpRequestIdentity()
            {
                Provider = $"{Supplier.Microtest}",
                Method = request?.Method?.ToString(),
                RequestUrl = request?.RequestUri,
                Identifier = null
            };

            return requestIdentity;
        }
    }
}
