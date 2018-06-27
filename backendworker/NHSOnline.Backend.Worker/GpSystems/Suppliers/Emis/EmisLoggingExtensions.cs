using System.Linq;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public static class EmisLoggingExtensions
    {
        public static void LogEmisUnknownError(this ILogger logger, EmisClient.EmisApiResponse response)
        {
            if (response == null)
            {
                logger.LogError("Call to EMIS returned a null response");
                return;
            }

            var emisMessages = response.ErrorResponse?.Exceptions?.Select(x => x.Message) ?? new[] { string.Empty };

            var message = string.Join(", ", emisMessages);

            logger.LogError(
                $"Call to EMIS returned an unanticipated error with status code: '{response.StatusCode}'. EMIS message: '{message}'");
        }

        public static void LogEmisResponseIsForbidden(this ILogger logger)
        {
            logger.LogError("Call to EMIS returned a forbidden response");
        }
    }
}