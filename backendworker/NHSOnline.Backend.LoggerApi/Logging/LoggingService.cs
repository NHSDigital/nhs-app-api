using Microsoft.Extensions.Logging;
using NHSOnline.Backend.LoggerApi.Areas.Logging.Models;

namespace NHSOnline.Backend.LoggerApi.Logging
{
    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger;
        }

        public void LogMessage(CreateLogRequest createLogRequest)
        {
            switch (createLogRequest.Level)
            {
                case Level.Debug:
                    _logger.LogDebug(createLogRequest.FormattedDebugLogMessage);
                    break;

                case Level.Error:
                    _logger.LogError(createLogRequest.FormattedErrorLogMessage);
                    break;

                default:
                    _logger.LogInformation(createLogRequest.FormattedInformationLogMessage);
                    break;
            }
        }
    }
}
