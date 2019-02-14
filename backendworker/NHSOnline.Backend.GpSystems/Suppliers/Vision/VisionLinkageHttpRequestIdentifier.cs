using System.Net.Http;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public class VisionLinkageHttpRequestIdentifier : IHttpRequestIdentifier
    {
        public HttpRequestIdentity Identify(HttpRequestMessage request)
        {
            var requestIdentity = new HttpRequestIdentity()
            {
                Provider = $"{Supplier.Vision}",
                Method = request?.Method?.ToString(),
                RequestUrl = request?.RequestUri,
                Identifier = null,
            };

            return requestIdentity;
        }
    }
}