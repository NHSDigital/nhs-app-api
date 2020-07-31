using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Logging.Scopes;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Logging
{
    internal sealed class CloudLogger: ILogger
    {
        private readonly string _name;
        private readonly ICloudLog _cloudLog;
        private readonly LoggerExternalScopeProvider _scopeProvider;
        private readonly INativeLog _nativeLog;

        internal CloudLogger(
            string name,
            ICloudLog cloudLog,
            LoggerExternalScopeProvider scopeProvider,
            INativeLog nativeLog)
        {
            _name = name;
            _cloudLog = cloudLog;
            _scopeProvider = scopeProvider;
            _nativeLog = nativeLog;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (ScopeIncludesNhsCloud(state))
            {
                return;
            }

            if (IsEnabled(logLevel)
                && TryCreateMessage(state, exception, formatter, out var message))
            {
                Task.Run(async () =>
                {
                    using (BeginScope(typeof(NhsCloudLogging)))
                    {
                        await _cloudLog.Log(logLevel, _name, message, DateTime.UtcNow).ResumeOnThreadPool();
                    }
                }).ContinueWith(task =>
                {
                    _nativeLog.Log(LogLevel.Error, "clougLogger", "Failed to log message");
                }, TaskScheduler.Default);
            }
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel > LogLevel.Information && logLevel < LogLevel.None;

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

        private bool ScopeIncludesNhsCloud<TState>(TState state)
        {
            var containsExpectedScope = false;
            _scopeProvider.ForEachScope((s, scopeLevel) =>
            {
                if (Equals(s, typeof(NhsCloudLogging)))
                {
                    containsExpectedScope = true;
                }
            }, state);

            return containsExpectedScope;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _scopeProvider.Push(state);
        }
    }
}