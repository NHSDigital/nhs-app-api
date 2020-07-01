using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Logging
{
    public interface INativeLog
    {
        void Log(LogLevel logLevel, string context, string message);
    }
}
