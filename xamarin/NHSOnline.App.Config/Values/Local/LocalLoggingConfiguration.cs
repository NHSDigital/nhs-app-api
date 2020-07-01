using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Config.Values.Local
{
    internal sealed class LocalLoggingConfiguration : ILoggingConfiguration
    {
        public LogLevel MinimumLogLevel => LogLevel.Trace;
        public LogLevel MinimumNativeLogLevel => LogLevel.Trace;
    }
}