using System;
using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Api.Logging
{
    internal sealed class CloudLoggingModel
    {
        public CloudLoggingModel(string message, LogLevel logLevel, DateTime timeStamp)
        {
            Message = message;
            TimeStamp = timeStamp;
            Level = logLevel;
        }

        public string Message { get; }
        public DateTime TimeStamp { get; }
        public LogLevel Level { get; }
    }
}