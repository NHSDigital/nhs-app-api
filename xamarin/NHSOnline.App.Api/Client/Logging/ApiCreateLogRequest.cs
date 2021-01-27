using System;
using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Api.Logging
{
    public sealed class ApiCreateLogRequest
    {
        private readonly CloudLoggingModel _model;

        public ApiCreateLogRequest(string message, LogLevel logLevel, DateTime timeStamp)
        {
            _model = new CloudLoggingModel(message, logLevel, timeStamp);
        }

        internal CloudLoggingModel CreateModel() => _model;
    }
}
