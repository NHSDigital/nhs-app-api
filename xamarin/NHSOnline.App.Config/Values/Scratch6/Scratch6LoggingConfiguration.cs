using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Config.Values.Scratch6
{
    internal sealed class Scratch6LoggingConfiguration : ILoggingConfiguration
    {
        public LogLevel MinimumLogLevel => LogLevel.Information;
        public LogLevel MinimumNativeLogLevel => LogLevel.Error;
    }
}