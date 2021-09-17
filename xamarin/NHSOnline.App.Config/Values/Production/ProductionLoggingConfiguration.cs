using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Config.Values.Production
{
    internal sealed class ProductionLoggingConfiguration : ILoggingConfiguration
    {
        public LogLevel MinimumLogLevel => LogLevel.Information;
        public LogLevel MinimumNativeLogLevel => LogLevel.Error;
    }
}