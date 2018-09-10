using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public static class TppLoggingExtensions
    {
        public static void LogTppUnknownError<T>(this ILogger logger, TppClient.TppApiObjectResponse<T> response)
        {
            if (response == null)
            {
                logger.LogError("Call to TPP returned a null response");
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
    }
}