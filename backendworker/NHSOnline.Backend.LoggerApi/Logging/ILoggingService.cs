using NHSOnline.Backend.LoggerApi.Areas.Logging.Models;

namespace NHOnline.Backend.LoggerApi.Logging
{
    public interface ILoggingService
    {
        void LogMessage(CreateLogRequest createLogRequest);
    }
}