using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Config.Values.OnboardingAos
{
    internal sealed class OnboardingAosLoggingConfiguration : ILoggingConfiguration
    {
        public LogLevel MinimumLogLevel => LogLevel.Information;
        public LogLevel MinimumNativeLogLevel => LogLevel.Information;
    }
}