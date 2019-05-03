using System.Net.Http;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public class EmisHttpRequestIdentifier : IHttpRequestIdentifier
    {
        public HttpRequestIdentity Identify(HttpRequestMessage request)
        {
            return new HttpRequestIdentity($"{Supplier.Emis}", request);
        }
    }
}