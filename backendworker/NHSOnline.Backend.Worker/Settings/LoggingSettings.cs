using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.Settings
{
    public class LoggingSettings
    {
        public LogLevel StandardLevel { get; set; } = LogLevel.Debug;
        public LogLevel ErrorLevel { get; set; } = LogLevel.Critical;
        public List<LogCensorFilter> CensorFilters { get; set; } = new List<LogCensorFilter>();

        public static LoggingSettings GetSettings(IConfiguration configuration)
        {
            var configurationSettings = configuration.GetSection("Logging:Application").Get<LoggingSettings>();
            return configurationSettings;
        }
    }
}
