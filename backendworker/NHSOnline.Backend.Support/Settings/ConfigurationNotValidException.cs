using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace NHSOnline.Backend.Support.Settings
{
    public class ConfigurationNotValidException : Exception
    {
        public ConfigurationNotValidException()
        {
        }

        public ConfigurationNotValidException(string invalidPropertyName)
            : base(string.Format(CultureInfo.InvariantCulture, ExceptionMessages.ConfigurationValueNotValid, invalidPropertyName))
        {
        }

        public ConfigurationNotValidException(string invalidPropertyName, Exception inner)
            : base(string.Format(CultureInfo.InvariantCulture, ExceptionMessages.ConfigurationValueNotValid, invalidPropertyName), inner)
        {
        }

        public ConfigurationNotValidException(IEnumerable<string> formattedErrorMessages)
            : base(string.Format(CultureInfo.InvariantCulture, string.Join(", ", formattedErrorMessages)))
        {
        }

        public ConfigurationNotValidException(string invalidPropertyName, string detail)
            : base(string.Format(CultureInfo.InvariantCulture, ExceptionMessages.ConfigurationValueNotValidWithDetail,
                invalidPropertyName, detail))
        {
        }

        protected ConfigurationNotValidException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}