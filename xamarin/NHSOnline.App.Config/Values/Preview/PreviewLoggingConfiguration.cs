using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Config.Values.Preview
{
    internal sealed class PreviewLoggingConfiguration : ILoggingConfiguration
    {
        public LogLevel MinimumLogLevel => LogLevel.Information;
        public LogLevel MinimumNativeLogLevel => LogLevel.Error;
    }
}