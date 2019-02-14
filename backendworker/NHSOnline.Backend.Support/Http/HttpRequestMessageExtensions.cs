using System.Net.Http;

namespace NHSOnline.Backend.Support.Http
{
    public static class HttpRequestMessageExtensions
    {
        public static void CheckAndSetCustomTimeout(this HttpRequestMessage request, int? customTimeout)
        {    
            if (customTimeout.HasValue)
            {
                request.Properties.Add(HttpRequestConstants.CustomTimeout, customTimeout);
            }
        }
    }
}