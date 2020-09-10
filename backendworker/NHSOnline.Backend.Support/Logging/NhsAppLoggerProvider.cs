using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support.Logging
{
    internal sealed class NhsAppLoggerProvider : ILoggerProvider
    {
        private readonly TextWriter _logWriter;
        private readonly LogLevel _minLogLevel;
        private readonly LogLevel _maxLogLevelLimit;
        private readonly LoggerExternalScopeProvider _scopeProvider;
        private readonly IEnumerable<LogCensorFilter> _regexFilterList;

        private bool _disposed;

        public NhsAppLoggerProvider(
            TextWriter logWriter,
            LogLevel minLogLevel,
            LogLevel maxLogLevelLimit = LogLevel.None,
            IEnumerable<LogCensorFilter> regexFilterList = null)
        {
            _logWriter = logWriter;
            _minLogLevel = minLogLevel;
            _maxLogLevelLimit = maxLogLevelLimit;
            _scopeProvider = new LoggerExternalScopeProvider();
            _regexFilterList = regexFilterList;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _logWriter.Dispose();
            _disposed = true;
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            return new NhsAppLogger(categoryName, _logWriter, _minLogLevel, _maxLogLevelLimit, _scopeProvider, _regexFilterList);
        }

        private sealed class NhsAppLogger : ILogger
        {
            private readonly TextWriter _textWriter;
            private readonly LogLevel _minLogLevel;
            private readonly LogLevel _maxLogLevelLimit;
            private readonly LoggerExternalScopeProvider _scopeProvider;
            private readonly string _categoryName;
            private readonly IEnumerable<LogCensorFilter> _regexFilterList;

            public NhsAppLogger(
                string categoryName,
                TextWriter logWriter,
                LogLevel minLogLevel,
                LogLevel maxLogLevelLimit,
                LoggerExternalScopeProvider scopeProvider,
                IEnumerable<LogCensorFilter> regexFilterList)
            {
                _textWriter = logWriter;
                _categoryName = categoryName;
                _minLogLevel = minLogLevel;
                _maxLogLevelLimit = maxLogLevelLimit;
                _scopeProvider = scopeProvider;
                _regexFilterList = regexFilterList;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (IsEnabled(logLevel))
                {
                    var exceptionMessage = string.Empty;
                    if (exception != null)
                    {
                        exceptionMessage = $"[Exception: {exception}] ";
                    }

                    var scope = GetScope(state);

                    _textWriter.WriteLine($"| {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss:fff} |" +
                                          (string.IsNullOrWhiteSpace(scope) ? string.Empty : $" {scope} |") +
                                          $" {_categoryName} |" +
                                          $" { logLevel } |" +
                                          $" {CensorLogMessage(formatter(state, exception))} {exceptionMessage}|");
                    _textWriter.Flush();
                }
            }

            private string CensorLogMessage(string state)
            {
                if (_regexFilterList != null)
                {
                    foreach (var filter in _regexFilterList)
                    {
                        state = filter.CensorContent(state);
                    }
                }

                return state;
            }

            private string GetScope<TState>(TState state)
            {
                var scope = new StringBuilder();
                _scopeProvider.ForEachScope((s, scopeLevel) =>
                {
                    if (scope.Length > 0)
                    {
                        scope.Append("=>");
                    }
                    scope.Append(s);
                }, state);

                return CensorLogMessage(scope.ToString());
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return _minLogLevel <= logLevel && _maxLogLevelLimit > logLevel;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return _scopeProvider.Push(state);
            }
        }
    }
}
