using System.Net.Http;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.CitizenId
{
    public class CitizenIdHttpRequestIdentifier : IHttpRequestIdentifier
    {
        public HttpRequestIdentity Identify(HttpRequestMessage request)
        {
            var requestIdentity = new HttpRequestIdentity()
            {
                Provider = "CitizenId",
                Method = request?.Method?.ToString(),
                RequestUrl = request?.RequestUri,
                Identifier = null
            };

            return requestIdentity;
        }
    }
}