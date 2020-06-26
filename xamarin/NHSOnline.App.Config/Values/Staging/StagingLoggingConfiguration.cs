using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Config.Values.Staging
{
    internal sealed class StagingLoggingConfiguration : ILoggingConfiguration
    {
        public LogLevel MinimumLogLevel => LogLevel.Information;
    }
}