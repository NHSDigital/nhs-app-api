using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    public class ErrorHandler : IErrorHandler
    {
        private readonly ILogger _logger;

        public ErrorHandler(ILogger logger)
        {
            _logger = logger;
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogCritical(string message)
        {
            _logger.LogCritical(message);
        }

        public void LogError(string message)
        {
            _logger.LogError(message);
        }
    }
}