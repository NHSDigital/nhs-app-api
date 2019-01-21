using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace NHSOnline.Backend.Support
{
    public class ValidateAndLog
    {
        private readonly ILogger _logger;
        private bool _argumentsAreValid;
        private readonly List<ArgumentException> _exceptions = new List<ArgumentException>();

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
                HandleMissingValue(name, options);
            }
            return this;
        }

        public ValidateAndLog IsNotNull<T>(T value, string name, ValidationOptions options = ValidationOptions.None)
        {
            if (value == null)
            {
                HandleMissingValue(name, options);
            }

            return this;
        }

        public ValidateAndLog IsValidOdsCode(string value, string name, ValidationOptions options = ValidationOptions.None)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return this;
            }

            var match = Regex.Match(value, Constants.OdsCodeFormats.GpPracticeEnglandWales);
            if (!match.Success)
            {
                _logger.LogError("Invalid Ods code supplied");
                HandleValueInvalid(name, options);
            }

            return this;
        }

        public ValidateAndLog IsSafeString(string value, string name, ValidationOptions options = ValidationOptions.None)
        {
            if(string.IsNullOrEmpty(value))
            {
                return this;
            }

            if (!value.IsSafeString())
            {
                _logger.LogError("Unsafe string provided, possible script injection attempt");
                HandleValueInvalid(name, options);
            }

            return this;
        }

        public ValidateAndLog IsStringValidLength(string value, int minLength, int maxLength, string name, ValidationOptions options = ValidationOptions.None)
        {
            if(string.IsNullOrWhiteSpace(value) && minLength > 0)
            {
                HandleMissingValue(name, options);
                return this;
            }

            if(string.IsNullOrWhiteSpace(value))
            {
                return this;
            }

            if (value.Length > maxLength || value.Length < minLength)
            {
                _logger.LogError("String provided is outside of valid range");
                HandleValueInvalid(name, options);
            }

            return this;
        }

        public ValidateAndLog IsListPopulated<T>(IEnumerable<T> value, string name, ValidationOptions options = ValidationOptions.None)
        {
            if(value == null || !value.Any())
            {
                _logger.LogError("Null or empty list provided but expected a populated list");
                HandleValueInvalid(name, options);
            }

            return this;
        }

        private void HandleMissingValue(string name, ValidationOptions options)
        {
            _logger.LogError(string.Format(CultureInfo.InvariantCulture, $"The value for '{name}' has not been supplied"));
            _argumentsAreValid = false;

            if((options & ValidationOptions.ThrowError) == ValidationOptions.ThrowError)
            {
                _exceptions.Add(new ArgumentNullException(name));
            }
        }

        private void HandleValueInvalid(string name, ValidationOptions options)
        {
            _argumentsAreValid = false;

            if ((options & ValidationOptions.ThrowError) == ValidationOptions.ThrowError)
            {
                _exceptions.Add(new ArgumentException(name));
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