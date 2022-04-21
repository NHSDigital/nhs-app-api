using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

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

        protected async Task<string> GetStringResponse(
            HttpResponseMessage responseMessage,
            ILogger logger,
            string context = "")
        {
            if (!HasSuccessResponse)
            {
                var message = context.IsNullOrEmpty()
                    ? $"{context} failed with status code {responseMessage?.StatusCode}"
                    : $"Request failed with status code {responseMessage?.StatusCode}";

                logger.LogWarning(message);

                if (!FormatResponseIfUnsuccessful)
                {
                    return null;
                }
            }

            return await responseMessage.StringResponse(logger);
        }
    }
}
