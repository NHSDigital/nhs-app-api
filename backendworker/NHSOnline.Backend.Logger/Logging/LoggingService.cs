using System.Text.Encodings.Web;
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
            string FormatLogMessage(string messageFieldName)
            {
                var encodedLogMessageValue = UrlEncoder.Default.Encode(createLogRequest.Message);
                var logMessageField = $@"client_{messageFieldName}_message=""{encodedLogMessageValue}""";
                var clientTimestampField = $@"client_timestamp=""{createLogRequest.TimeStamp:yyyy-MM-dd HH:mm:ss}""";

                return $"{logMessageField} {clientTimestampField}";
            }

            switch (createLogRequest.Level)
            {
                case Level.Debug:
                    _logger.LogDebug(FormatLogMessage("debug"));
                    break;

                case Level.Error:
                    _logger.LogError(FormatLogMessage("error"));
                    break;

                default:
                    _logger.LogInformation(FormatLogMessage("information"));
                    break;
            }
        }
    }
}
