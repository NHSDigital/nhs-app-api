using System.Net.Http;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest
{
    public class MicrotestHttpRequestIdentifier : IHttpRequestIdentifier
    {
        public HttpRequestIdentity Identify(HttpRequestMessage request)
        {
            return new HttpRequestIdentity($"{Supplier.Microtest}", request);
        }
    }
}
