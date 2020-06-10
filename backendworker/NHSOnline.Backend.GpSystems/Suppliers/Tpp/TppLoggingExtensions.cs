using System;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
{
    public static class TppLoggingExtensions
    {
        public static void LogTppErrorResponse<T>(this ILogger logger, TppApiObjectResponse<T> response)
        {
            if (IsResponseNull(logger, response))
            {
                return;
            }

            try
            {
                var message = response.ErrorResponse?.TechnicalMessage ?? response.ErrorResponse?.UserFriendlyMessage ?? string.Empty;
                if (!string.IsNullOrEmpty(message))
                {
                    logger.LogError("TPP Response: " + message);
                }
            }
            catch (Exception e)
            {
                logger.LogError("Unable to serialize and log TPP response");
                logger.LogError(e.StackTrace);
            }
        }

        public static void LogTppUnknownError<T>(this ILogger logger, TppApiObjectResponse<T> response)
        {
            if (IsResponseNull(logger, response))
            {
                return;
            }

            var message = response.ErrorResponse?.TechnicalMessage ?? response.ErrorResponse?.UserFriendlyMessage ?? string.Empty;

            logger.LogError(
                $"Call to TPP returned an unanticipated error with status code: '{response.StatusCode}'. TPP message: '{message}'");
        }

        public static void LogTppResponseAccessIsForbidden(this ILogger logger)
        {
            logger.LogError("Call to TPP returned a forbidden response");
        }

        private static bool IsResponseNull<T>(ILogger logger, TppApiObjectResponse<T> response)
        {
            if (null != response)
            {
                return false;
            }

            logger.LogError("Call to TPP returned a null response");
            return true;
        }
    }
}