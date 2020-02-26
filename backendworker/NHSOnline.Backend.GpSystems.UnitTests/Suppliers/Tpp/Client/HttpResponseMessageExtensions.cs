using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Client
{
    internal static class HttpResponseMessageExtensions
    {
        public static HttpResponseMessage StringContent(this HttpResponseMessage message, string content)
        {
            message.Content = new StringContent(content);
            return message;
        }

        public static HttpResponseMessage AddHeader(this HttpResponseMessage message, string name, string value)
        {
            message.Headers.Add(name, value);
            return message;
        }

        public static Task<HttpResponseMessage> ReturnTask(this HttpResponseMessage message)
        {
            return Task.FromResult(message);
        }

        public static HttpResponseMessage SetStatusCode(this HttpResponseMessage message, HttpStatusCode code)
        {
            message.StatusCode = code;
            return message;
        }
    }
}