using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Config.Values.OnboardingSandpit
{
    internal sealed class OnboardingSandpitLoggingConfiguration : ILoggingConfiguration
    {
        public LogLevel MinimumLogLevel => LogLevel.Information;
        public LogLevel MinimumNativeLogLevel => LogLevel.Information;
    }
}