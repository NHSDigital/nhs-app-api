using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;

namespace NHSOnline.IntegrationTests.UI
{
    internal sealed class MocksLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, LogDeliveryLogger> _loggers = new ConcurrentDictionary<string, LogDeliveryLogger>();
        public event EventHandler<string>? Logged;

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string name)
        {
            return _loggers.GetOrAdd(name, loggerName => new LogDeliveryLogger(loggerName, this));
        }

        private void Log(string log)
        {
            Logged?.Invoke(this, log);
        }

        private sealed class LogDeliveryLogger : ILogger
        {
            private readonly string _name;
            private readonly MocksLoggerProvider _mocksLoggerProvider;

            public LogDeliveryLogger(string name, MocksLoggerProvider mocksLoggerProvider)
            {
                _name = name;
                _mocksLoggerProvider = mocksLoggerProvider;
            }

            public IDisposable? BeginScope<TState>(TState _) => null;

            public bool IsEnabled(LogLevel logLevel)
            {
                return logLevel != LogLevel.None;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                var message = formatter(state, exception);
                if (!string.IsNullOrEmpty(message) || exception != null)
                {
                    WriteMessage(logLevel, _name, message, exception);
                }
            }

            private void WriteMessage(LogLevel logLevel, string name, string message, Exception exception)
            {
                var logBuilder = new StringBuilder();

                logBuilder.Append(DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture));
                logBuilder.Append(" ");
                logBuilder.Append(name);
                logBuilder.Append(" ");
                logBuilder.Append(logLevel.ToString());
                logBuilder.Append(" ");

                if (!string.IsNullOrEmpty(message))
                {
                    logBuilder.Append(message);
                }

                if (exception != null)
                {
                    logBuilder.AppendLine(exception.ToString());
                }

                var logEntry = logBuilder.ToString();

                _mocksLoggerProvider.Log(logEntry);
            }
        }
    }
}