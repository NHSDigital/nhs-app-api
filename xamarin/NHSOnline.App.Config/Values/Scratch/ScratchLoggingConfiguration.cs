using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Config.Values.Scratch
{
    internal sealed class ScratchLoggingConfiguration : ILoggingConfiguration
    {
        public LogLevel MinimumLogLevel => LogLevel.Information;
        public LogLevel MinimumNativeLogLevel => LogLevel.Error;
    }
}