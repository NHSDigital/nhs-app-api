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

        private const string NotSuppliedErrorLogMessage = "The value for '{0}' has not been supplied";

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
                HandleNullError(name, options);
            }
            return this;
        }

        public ValidateAndLog IsNotNull<T>(T value, string name, ValidationOptions options = ValidationOptions.None)
        {
            if (value == null)
            {
                HandleNullError(name, options);
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
                HandleError(name, "Invalid Ods code supplied", options);
            }

            return this;
        }

        public ValidateAndLog IsSafeString(string value, string name, ValidationOptions options = ValidationOptions.None)
        {
            if (!string.IsNullOrEmpty(value) && !value.IsSafeString())
            {
                HandleError(name, "Unsafe string provided, possible script injection attempt", options);
            }

            return this;
        }

        public ValidateAndLog IsStringValidLength(string value, int minLength, int maxLength, string name, ValidationOptions options = ValidationOptions.None)
        {
            if(string.IsNullOrWhiteSpace(value) && minLength > 0)
            {
                HandleNullError(name, options);
                return this;
            }
            
            if (!string.IsNullOrWhiteSpace(value) && (value.Length > maxLength || value.Length < minLength))
            {
                HandleError(name, "String provided is outside of valid range", options);
            }

            return this;
        }

        public ValidateAndLog IsListPopulated<T>(IEnumerable<T> value, string name, ValidationOptions options = ValidationOptions.None)
        {
            if(value == null || !value.Any())
            {
                HandleError(name, "Null or empty list provided but expected a populated list", options);
            }

            return this;
        }

        public ValidateAndLog HasValue<T>(T value, string name, ValidationOptions options)
        {
            if (value.Equals(default(T)))
            {
                HandleError(name, NotSuppliedErrorLogMessage, options);
            }
            
            return this;
        }

        private void HandleNullError(string name, ValidationOptions options)
            => HandleError(name, options, NotSuppliedErrorLogMessage, paramName => new ArgumentNullException(paramName));
        
        private void HandleError(string name, string message, ValidationOptions options)
            => HandleError(name, options, message, paramName => new ArgumentException(paramName, paramName));
        
        private void HandleError(string name, ValidationOptions options, string message, Func<string, ArgumentException> createException)
        {
            _logger.LogError(string.Format(CultureInfo.InvariantCulture, message, name));
            _argumentsAreValid = false;

            if((options & ValidationOptions.ThrowError) == ValidationOptions.ThrowError)
            {
                _exceptions.Add(createException(name));
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