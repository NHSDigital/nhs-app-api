using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;

namespace NHSOnline.Backend.Worker.Support.Logging
{
    public class HttpContexedLoggerProvider : ILoggerProvider
    {
        private readonly TextWriter _logwriter;
        private readonly LogLevel _minLogLevel;
        private readonly LogLevel _maxLogLevelLimit;
        private readonly LoggerExternalScopeProvider _scopeProvider;
        
        public HttpContexedLoggerProvider
        (
            TextWriter logwriter, 
            LogLevel defualtMinLogLevel, 
            string configuredMinLogLevel = null,
            LogLevel defualtMaxLogLevelLimit = LogLevel.None,
            string configuredMaxLogLimit = null
        )
        {
            _logwriter = logwriter;
            _minLogLevel = ParsedLogLevel(configuredMinLogLevel, defualtMinLogLevel);
            _maxLogLevelLimit = ParsedLogLevel(configuredMaxLogLimit, defualtMaxLogLevelLimit);
            _scopeProvider = new LoggerExternalScopeProvider();
        }

        private LogLevel ParsedLogLevel(string configuredLogLevel, LogLevel defualtLogLevel)
        {
            LogLevel logLevel = defualtLogLevel;
            if (string.IsNullOrEmpty(configuredLogLevel) == false)
            {
                if (Enum.TryParse(configuredLogLevel, out logLevel) == false)
                {
                    throw new Exception(string.Format(ExceptionMessages.InvalidLogLevel, configuredLogLevel));
                }
            }

            return logLevel;

        }

        public void Dispose() { }

        public ILogger CreateLogger(string categoryName)
        {
            return new HttpContexedLogger(categoryName, _logwriter, _minLogLevel, _maxLogLevelLimit, _scopeProvider);
        }
     
        private class HttpContexedLogger : ILogger
        {
            private readonly TextWriter _textWriter;
            private readonly LogLevel _minLogLevel;
            private readonly LogLevel _maxLogLevelLimit;
            private readonly LoggerExternalScopeProvider _scopeProvider;
            private readonly string _categoryName;

            public HttpContexedLogger(string categoryName, TextWriter logWriter, LogLevel minLogLevel, LogLevel maxLogLevelLimit, LoggerExternalScopeProvider scopeProvider)
            {
                _textWriter = logWriter;
                _categoryName = categoryName;
                _minLogLevel = minLogLevel;
                _maxLogLevelLimit = maxLogLevelLimit;
                _scopeProvider = scopeProvider;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (IsEnabled(logLevel))
                {
                    lock (_textWriter)
                    {
                        _textWriter.WriteLine($"| {DateTime.Now.ToString("yyy-MM-dd HH:mm:ss:fff")} | { GetScope(state) } | {_categoryName} | { logLevel } | {formatter(state, exception)} |");
                        _textWriter.Flush();
                    }
                }
            } 

            private string GetScope<TState>(TState state)
            {
                var scope = new StringBuilder();
                _scopeProvider.ForEachScope((s, scopeLevel) => 
                {
                    if(scope.Length >0)
                    {
                        scope.Append("=>");
                    }
                    scope.Append(s.ToString());
                }, state);
                
                return scope.ToString();
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return _minLogLevel <= logLevel && _maxLogLevelLimit > logLevel;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                if (state.GetType().IsSubclassOf(typeof(HttpContext)))
                {
                    return _scopeProvider.Push(new HttpContextLoggerScope(state as HttpContext));
                }
                    
                return _scopeProvider.Push(state);
            }
        }
    }
}
