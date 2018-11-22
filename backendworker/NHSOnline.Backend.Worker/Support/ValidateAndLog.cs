using System;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Support
{
    public class ValidateAndLog
    {
        private readonly ILogger _logger;
        private bool _argumentsAreValid;
        private string _errorMessage = "The value for '{0}' has not been supplied";
        public ValidateAndLog(ILogger logger)
        {
            _logger = logger;
            _argumentsAreValid = true;
        }

        public ValidateAndLog IsNotNullOrWhitespace(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _logger.LogError(string.Format(CultureInfo.InvariantCulture, _errorMessage, name));
                _argumentsAreValid = false;
            }
            return this;
        }

        public ValidateAndLog IsNotNull(DateTime? value, string name)
        {
            if (value == null)
            {
                _logger.LogError(string.Format(CultureInfo.InvariantCulture, _errorMessage, name));
                _argumentsAreValid = false;
            }
            return this;
        }

        public bool IsValid()
        {
            return _argumentsAreValid;
        }
    }
}