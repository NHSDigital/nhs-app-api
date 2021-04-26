using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Config.Values.Scratch18
{
    internal sealed class Scratch18LoggingConfiguration : ILoggingConfiguration
    {
        public LogLevel MinimumLogLevel => LogLevel.Information;
        public LogLevel MinimumNativeLogLevel => LogLevel.Error;
    }
}