using System.Linq;
using System.Net.Http;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public class VisionPFSHttpRequestIdentifier : IHttpRequestIdentifier
    {
        public HttpRequestIdentity Identify(HttpRequestMessage request)
        {
            string identifier = null;
            if (request?.Headers != null &&
                request.Headers.Contains(Constants.VisionConstants.RequestIdentifierHeader))
            {
                request.Headers.TryGetValues(Constants.VisionConstants.RequestIdentifierHeader, out var values);
                identifier = values.FirstOrDefault();
            }
            
            return 
                new HttpRequestIdentity($"{Supplier.Vision}", request)
                .SetUpStreamIdentifier(identifier);
        }
    }
}