using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace NHSOnline.Backend.Support.Settings
{
    public class ConfigurationNotFoundException : Exception
    {
        public ConfigurationNotFoundException()
        {
        }

        public ConfigurationNotFoundException(string name)
            : base(string.Format(CultureInfo.InvariantCulture, ExceptionMessages.ConfigurationValueNotFound, name))
        {
        }

        public ConfigurationNotFoundException(string name, Exception inner)
            : base(string.Format(CultureInfo.InvariantCulture, ExceptionMessages.ConfigurationValueNotFound, name), inner)
        {
        }

        protected ConfigurationNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

    }
}
