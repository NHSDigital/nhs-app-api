using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<string> StringResponse(this HttpResponseMessage responseMessage, ILogger logger)
        {
            var stringResponse = responseMessage.Content != null
                ? await responseMessage.Content.ReadAsStringAsync()
                : null;

            if (string.IsNullOrEmpty(stringResponse))
            {
                logger.LogError($"Response with status code {responseMessage.StatusCode} and no body");
            }

            return stringResponse;
        }
    }
}