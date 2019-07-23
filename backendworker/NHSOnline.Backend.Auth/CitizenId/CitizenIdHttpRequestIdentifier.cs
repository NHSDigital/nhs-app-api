using System.Net.Http;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public class CitizenIdHttpRequestIdentifier : IHttpRequestIdentifier
    {
        public HttpRequestIdentity Identify(HttpRequestMessage request)
        {
            return new HttpRequestIdentity("CitizenId", request);
        }
    }
}