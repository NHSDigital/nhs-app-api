using System;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace NHSOnline.Backend.Worker.Support.Logging
{
    public static class LoggingExtensions
    {
        public static void LogHttpRequest(this ILogger logger, HttpRequestMessage requestMessage)
        {
            logger.LogInformation($"Sending request to: RequestUri: {RemoveQueryString(requestMessage)}");
        }

        public static void LogHttpResponse(this ILogger logger, HttpRequestMessage requestMessage, HttpResponseMessage responseMessage)
        {
            logger.LogInformation($"Received response from {RemoveQueryString(requestMessage)} with code {responseMessage.StatusCode}");
        }

        private static string RemoveQueryString(HttpRequestMessage requestMessage)
        {
            string uri = requestMessage.RequestUri?.ToString() ?? "";
            int startOfQueryString = uri.IndexOf('?', StringComparison.Ordinal);
            return startOfQueryString > 0 ? uri.Substring(0, startOfQueryString) + "?*****" : uri;
        }

        public static void LogEnter<T>(this ILogger<T> logger, string methodName)
        {
            logger.LogDebug($"Entering {methodName}");
        }
        
        public static void LogExit<T>(this ILogger<T> logger, string methodName)
        {
            logger.LogDebug($"Exiting {methodName}");
        }
    }
}
