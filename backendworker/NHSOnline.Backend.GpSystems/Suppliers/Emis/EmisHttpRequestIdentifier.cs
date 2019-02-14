using System.Net.Http;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public class EmisHttpRequestIdentifier : IHttpRequestIdentifier
    {
        public HttpRequestIdentity Identify(HttpRequestMessage request)
        {
            var requestIdentity = new HttpRequestIdentity()
            {
                Provider = $"{Supplier.Emis}",
                Method = request?.Method?.ToString(),
                RequestUrl = request?.RequestUri,
                Identifier = null
            };

            return requestIdentity;
        }
    }
}