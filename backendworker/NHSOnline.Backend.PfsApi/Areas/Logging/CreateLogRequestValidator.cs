using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Logger.Areas.Logging.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Logging
{
    public class CreateLogRequestValidator
    {
        private readonly ILogger _logger;

        public CreateLogRequestValidator(ILogger logger)
        {
            _logger = logger;
        }

        public bool Validate(CreateLogRequest createLogRequest)
        {
            return new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(createLogRequest.Message, nameof(createLogRequest.Message))
                .IsNotNull(createLogRequest.TimeStamp, nameof(createLogRequest.TimeStamp))
                .IsNotNull(createLogRequest.Level, nameof(createLogRequest.Level))
                .IsValid();
        }
    }
}
