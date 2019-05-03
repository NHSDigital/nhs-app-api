using System.Net.Http;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public class VisionLinkageHttpRequestIdentifier : IHttpRequestIdentifier
    {
        public HttpRequestIdentity Identify(HttpRequestMessage request)
        {
            return new HttpRequestIdentity($"{Supplier.Vision}", request);
        }
    }
}