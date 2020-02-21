using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NHSOnline.Backend.Support.Logging
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

        public static void LogEnter<T>(this ILogger<T> logger, [System.Runtime.CompilerServices.CallerMemberName] string methodName = "")
        {
            logger.LogDebug($"Entering {methodName}");
        }
        
        public static void LogExit<T>(this ILogger<T> logger, [System.Runtime.CompilerServices.CallerMemberName] string methodName = "")
        {
            logger.LogDebug($"Exiting {methodName}");
        }

        public static void LogExitWith<T>(this ILogger<T> logger, string message, [System.Runtime.CompilerServices.CallerMemberName] string methodName = "")
        {
            logger.LogDebug($"Exiting {methodName} with {message}");
        }

        public static void LogFullException<T>(this ILogger<T> logger, Exception exception)
        {
            if (exception == null) return;
            logger.LogError(exception, "Exception thrown");
            logger.LogInnerException(exception);
        }
        
        public static void LogInformationKeyValuePairs(this ILogger logger, string title, IDictionary<string, string> kvp)
        {
            var items = kvp.Select(x => $"{x.Key}={x.Value}");
            var message = $"{title}: {string.Join(" ", items)}";
            logger.LogInformation(message);
        }

        public static void LogVersion(this ILogger logger, HttpContext context, string apiAppName, string apiAppVersion)
        {
            string webAppVersion = context.Request.Headers[Constants.HttpHeaders.WebAppVersion];
            string nativeAppVersion = context.Request.Headers[Constants.HttpHeaders.NativeAppVersion];

            if (string.IsNullOrEmpty(webAppVersion))
            {
                return;
            }

            var logMessageStringBuilder = new StringBuilder();

            logMessageStringBuilder.Append(
                $"Beginning HTTP Request. {apiAppName} version: {apiAppVersion}. Web App Version: {webAppVersion}.");

            if (!string.IsNullOrEmpty(nativeAppVersion))
            {
                logMessageStringBuilder.Append($" Native App Version: {nativeAppVersion}.");
            }

            logger.LogInformation(logMessageStringBuilder.ToString());
        }

        public static void LogModelStateValidationFailure<T>(this ILogger<T> logger, ModelStateDictionary modelState)
        {
            logger.LogWarning("Model state validation failed: {0}", 
                modelState.Values.SelectMany(x=>x.Errors).Select(x=>x.ErrorMessage));
        }

        private static void LogInnerException<T>(this ILogger<T> logger, Exception exception, int level = 0)
        {
            if (exception?.InnerException == null) return;
            level++;
            logger.LogError(exception.InnerException, $"Inner exception #{level}");
            logger.LogInnerException(exception.InnerException, level);
        }
    }
}
