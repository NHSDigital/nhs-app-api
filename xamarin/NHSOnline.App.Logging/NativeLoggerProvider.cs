using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Logging
{
    internal sealed class NativeLoggerProvider: ILoggerProvider
    {
        private readonly LogLevel _minimumLevel;
        private readonly INativeLog _nativeLog;

        private readonly ConcurrentDictionary<string, NativeLogger> _loggers = new ConcurrentDictionary<string, NativeLogger>(StringComparer.Ordinal);

        public NativeLoggerProvider(LogLevel minimumLevel, INativeLog nativeLog)
        {
            _minimumLevel = minimumLevel;
            _nativeLog = nativeLog;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new NativeLogger(name, _minimumLevel, _nativeLog));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}