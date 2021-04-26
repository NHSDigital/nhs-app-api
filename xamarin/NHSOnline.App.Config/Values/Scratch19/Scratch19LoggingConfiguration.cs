using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Config.Values.Scratch19
{
    internal sealed class Scratch19LoggingConfiguration : ILoggingConfiguration
    {
        public LogLevel MinimumLogLevel => LogLevel.Information;
        public LogLevel MinimumNativeLogLevel => LogLevel.Error;
    }
}