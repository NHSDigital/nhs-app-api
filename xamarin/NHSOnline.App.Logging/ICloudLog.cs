using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Logging
{
    public interface ICloudLog
    {
        Task Log(LogLevel logLevel, string context, string message, DateTime timeStamp);
    }
}
