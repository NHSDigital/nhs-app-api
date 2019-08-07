using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace NHSOnline.Backend.Support.Settings
{
    public class ConfigurationNotValidException : Exception
    {
        public ConfigurationNotValidException()
        {
        }

        public ConfigurationNotValidException(string name)
            : base(string.Format(CultureInfo.InvariantCulture, ExceptionMessages.ConfigurationValueNotValid, name))
        {
        }

        public ConfigurationNotValidException(string name, Exception inner)
            : base(string.Format(CultureInfo.InvariantCulture, ExceptionMessages.ConfigurationValueNotValid, name), inner)
        {
        }

        protected ConfigurationNotValidException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

    }
}