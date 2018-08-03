using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.Settings
{
    public class ConfigurationNotFoundException : Exception
    {
        public ConfigurationNotFoundException()
        {
        }

        public ConfigurationNotFoundException(string message) 
            : base(string.Format(CultureInfo.InvariantCulture, ExceptionMessages.ConfigurationValueNotFound, message))
        {
        }

        public ConfigurationNotFoundException(string message, Exception inner) 
            : base(string.Format(CultureInfo.InvariantCulture, ExceptionMessages.ConfigurationValueNotFound, message), inner)
        {
        }

        protected ConfigurationNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

    }
}
