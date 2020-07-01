using System;
using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Logging
{
    internal sealed class NativeLogger: ILogger
    {
        private readonly string _name;
        private readonly LogLevel _minimumLevel;
        private readonly INativeLog _nativeLog;

        internal NativeLogger(string name, LogLevel minimumLevel, INativeLog nativeLog)
        {
            _name = name;
            _minimumLevel = minimumLevel;
            _nativeLog = nativeLog;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            _nativeLog.Log(logLevel, _name, formatter(state, exception));
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel >= _minimumLevel;

        public IDisposable? BeginScope<TState>(TState state) => null;
    }
}