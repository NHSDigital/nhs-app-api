using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support.Logging
{
    public static class LoggingExtensions
    {
        public static void LogEnter(this ILogger logger, [CallerMemberName] string methodName = "")
        {
            logger.LogDebug($"Entering {methodName}");
        }

        public static void LogExit(this ILogger logger, [CallerMemberName] string methodName = "")
        {
            logger.LogDebug($"Exiting {methodName}");
        }

        public static void LogExitWith<T>(this ILogger<T> logger, string message, [CallerMemberName] string methodName = "")
        {
            logger.LogDebug($"Exiting {methodName} with {message}");
        }

        public static void LogInformationKeyValuePairs(this ILogger logger, string title, IDictionary<string, string> kvp)
        {
            var items = kvp.Select(x =>
                $"{x.Key}={x.Value.EnquoteStringIfItContainsWhitespace()}");
            var message = $"{title}: {string.Join(" ", items)}";
            logger.LogInformation(message);
        }

        public static void LogAppointmentReasonInformation(this ILogger logger, string bookingReason)
        {
            bookingReason = bookingReason ?? string.Empty;
            var kvp = new Dictionary<string, string>
            {
                { "More than one character in booking reason", $"{bookingReason.Length >= 1}" },
                { "Characters entered in booking reason",  $"{bookingReason.Length }" }
            };

            logger.LogInformationKeyValuePairs("Appointments Booking Reason Info", kvp);
        }

        public static void LogSpecialRequestInformation(this ILogger logger, string requestComment)
        {
            requestComment = requestComment ?? string.Empty;
            var moreThanOneCharacter = requestComment.Length >= 1;


            var encodedSpecialRequestLength = requestComment.FindNewlineStringEncodedLength(Constants.EncodedCharacterValues.NewLineEncodedValue);

            var kvp = new Dictionary<string, string>
            {
                { "More than one character in special request", $"{moreThanOneCharacter}" },
                { "Characters entered in special request",  $"{encodedSpecialRequestLength }" }
            };

            logger.LogInformationKeyValuePairs("Special Request Prescriptions", kvp);
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


        public static void LogFieldCharacterLimitExceeded(this ILogger logger, string pageName, string fieldName, string description, int newlines)
        {
            var message = ($"{pageName} - {fieldName} - {description} - {newlines} new lines added");
            logger.LogInformation(message);
        }

    }
}
