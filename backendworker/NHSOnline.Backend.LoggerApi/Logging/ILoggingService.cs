using NHSOnline.Backend.LoggerApi.Areas.Logging.Models;

namespace NHSOnline.Backend.LoggerApi.Logging
{
    public interface ILoggingService
    {
        void LogMessage(CreateLogRequest createLogRequest);
    }
}