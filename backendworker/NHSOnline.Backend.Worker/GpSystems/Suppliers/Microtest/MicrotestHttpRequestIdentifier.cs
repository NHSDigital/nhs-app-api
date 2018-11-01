using System.Net.Http;
using NHSOnline.Backend.Worker.Support.Http;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest
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
