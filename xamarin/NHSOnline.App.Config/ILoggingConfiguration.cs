using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Config
{
    public interface ILoggingConfiguration
    {
        LogLevel MinimumLogLevel { get; }
    }
}