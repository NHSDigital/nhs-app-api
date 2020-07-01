using System;
using System.Diagnostics.CodeAnalysis;
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

            if (TryCreateMessage(state, exception, formatter, out var message))
            {
                _nativeLog.Log(logLevel, _name, message);
            }
        }

        private static bool TryCreateMessage<TState>(
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter,
            [NotNullWhen(true)] out string? message)
        {
            message = formatter(state, exception);

            // The default formatter ignores the exception: https://github.com/aspnet/Logging/issues/442
            if (message == null)
            {
                message = exception?.ToString();
            }
            else if (exception != null)
            {
                message = $"{message}{Environment.NewLine}{exception}";
            }

            return message != null;
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel >= _minimumLevel;

        public IDisposable? BeginScope<TState>(TState state) => null;
    }
}