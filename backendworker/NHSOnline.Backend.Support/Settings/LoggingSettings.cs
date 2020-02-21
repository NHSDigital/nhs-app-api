using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.Support.Settings
{
    internal sealed class LoggingSettings
    {
        public LogLevel StandardLevel { get; set; } = LogLevel.Debug;
        public LogLevel ErrorLevel { get; set; } = LogLevel.Critical;
        public List<LogCensorFilter> CensorFilters { get; set; } = new List<LogCensorFilter>();

        internal static LoggingSettings GetSettings(IConfiguration configuration)
        {
            var configurationSettings = configuration.GetSection("Logging:Application").Get<LoggingSettings>();
            return configurationSettings;
        }
    }
}
