using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.LoggerApi.Areas.Logging.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.LoggerApi.Areas.Logging
{
    public class CreateLogRequestValidator
    {
        private readonly ILogger _logger;
        
        public CreateLogRequestValidator(ILogger logger)
        {
            _logger = logger;
        }
        
        public bool ValidateAndSanitize(CreateLogRequest createLogRequest)
        {
            var isValid = new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(createLogRequest.Message, nameof(createLogRequest.Message))
                .IsNotNull(createLogRequest.TimeStamp, nameof(createLogRequest.TimeStamp))
                .IsNotNull(createLogRequest.Level, nameof(createLogRequest.Level))
                .IsValid();

            if (isValid)
            {
                createLogRequest.Message = SanitizeMessage(createLogRequest.Message);
            }

            return isValid;
        }

        private static string SanitizeMessage(string errorMessage)
        {
            return Regex.Replace(errorMessage, Constants.Regex.ClientSideLogMessageWhitelist, string.Empty);
        }
    }
}
