using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support
{
    public abstract class ApiResponse
    {
        protected ApiResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; set; }

        public abstract bool HasSuccessResponse { get; }

        protected abstract bool FormatResponseIfUnsuccessful { get; }

        protected async Task<string> GetStringResponse(HttpResponseMessage responseMessage, ILogger logger)
        {
            if (!HasSuccessResponse)
            {
                logger.LogWarning($"Request failed with status code {responseMessage.StatusCode}");

                if (!FormatResponseIfUnsuccessful)
                {
                    return null;
                }
            }

            return await FormatStringResponse(responseMessage, logger);
        }

        private static async Task<string> FormatStringResponse(HttpResponseMessage responseMessage, ILogger logger)
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
