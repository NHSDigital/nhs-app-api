using System.Net;
using System.Net.Http;
using System.Reflection;

namespace UnitTestHelper
{
    public static class HttpWebResponseHelper
    {
        public static HttpWebResponse CreateFromStatusCode(HttpStatusCode statusCode)
        {
            var response = new HttpWebResponse();
#pragma warning disable CA2000 // Dispose objects before losing scope
            var message = new HttpResponseMessage(statusCode);
#pragma warning restore CA2000 // Dispose objects before losing scope

            var fieldStatusCode = response.GetType().GetField("_httpResponseMessage",
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance);
            fieldStatusCode.SetValue(response, message);

            return response;
        }
    }
}