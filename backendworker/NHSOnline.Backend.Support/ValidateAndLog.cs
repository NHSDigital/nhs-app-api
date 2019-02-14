using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support
{
    public class ValidateAndLog
    {
        private readonly ILogger _logger;
        private bool _argumentsAreValid;
        private readonly List<ArgumentNullException> _exceptions = new List<ArgumentNullException>();

        [Flags]
        public enum ValidationOptions
        {
            None = 0,
            ThrowError = 1,
        };

        public ValidateAndLog(ILogger logger)
        {
            _logger = logger;
            _argumentsAreValid = true;
        }

        public ValidateAndLog IsNotNullOrWhitespace(string value, string name, ValidationOptions options = ValidationOptions.None)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                HandleError(name, options);
            }
            return this;
        }

        public ValidateAndLog IsNotNull<T>(T value, string name, ValidationOptions options = ValidationOptions.None)
        {
            if (value == null)
            {
                HandleError(name, options);
            }

            return this;
        }

        private void HandleError(string name, ValidationOptions options)
        {
            _logger.LogError(string.Format(CultureInfo.InvariantCulture, "The value for '{0}' has not been supplied", name));
            _argumentsAreValid = false;

            if((options & ValidationOptions.ThrowError) == ValidationOptions.ThrowError)
            {
                _exceptions.Add(new ArgumentNullException(name));
            }
        }

        public bool IsValid()
        {
            if (_exceptions.Count > 0)
            {
                throw new AggregateException(_exceptions);
            }
            return _argumentsAreValid;
        }
    }
}