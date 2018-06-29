using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.Settings
{
    public class ConfigurationNotFoundException : Exception
    {
        public ConfigurationNotFoundException(string configurationSetting)
        : base(string.Format(ExceptionMessages.ConfigurationValueNotFound, configurationSetting))
        { }
    }
}
