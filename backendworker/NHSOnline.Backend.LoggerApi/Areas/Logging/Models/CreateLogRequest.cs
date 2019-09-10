using System;

namespace NHSOnline.Backend.LoggerApi.Areas.Logging.Models
{
    public class CreateLogRequest
    {
        public DateTimeOffset TimeStamp { get; set; }
        
        public Level? Level { get; set; }
        
        public string Message { get; set; }

        public string FormattedLogMessage
        {
            get
            {
                return $"client_error_message={Message} client_timestamp={TimeStamp:yyyy-MM-dd HH:mm:ss}";
            }
        }
    }
}
