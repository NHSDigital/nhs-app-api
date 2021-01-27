using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Logging
{
    public sealed class CloudLoggerProvider: ILoggerProvider
    {
        private readonly ICloudLog _cloudLog;
        private readonly INativeLog _nativeLog;
        private readonly LoggerExternalScopeProvider _scopeProvider;

        private readonly ConcurrentDictionary<string, CloudLogger> _loggers = new ConcurrentDictionary<string, CloudLogger>(StringComparer.Ordinal);

        public CloudLoggerProvider(
            ICloudLog cloudLog,
            INativeLog nativeLog)
        {
            _cloudLog = cloudLog;
            _scopeProvider = new LoggerExternalScopeProvider();
            _nativeLog = nativeLog;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(
                categoryName,
                name => new CloudLogger(
                    name,
                    _cloudLog,
                    _scopeProvider,
                    _nativeLog));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}