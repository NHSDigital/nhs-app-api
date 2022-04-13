using NHSOnline.Backend.Logger.Areas.Logging.Models;

namespace NHSOnline.Backend.Logger.Logging
{
    public interface ILoggingService
    {
        void LogMessage(CreateLogRequest createLogRequest);
    }
}