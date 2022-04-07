using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace NHSOnline.MetricLogFunctionApp.Compute.Logging;

internal class ComputeLoggerState
{
    private LoggerState _loggerState = new NullState();

    internal void Started(string tableName, DateTime startDateTime, DateTime endDateTime)
        => _loggerState = new InitialisedLoggerState(tableName, startDateTime, endDateTime);

    internal void Log(ILogger logger, LogLevel logLevel, string format, params object[] args)
        => _loggerState.Log(logger, logLevel, format, args);

    private abstract class LoggerState
    {
        internal abstract void Log(ILogger logger, LogLevel logLevel, string format, params object[] args);
    }

    private sealed class NullState : LoggerState
    {
        internal override void Log(ILogger logger, LogLevel logLevel, string format, params object[] args)
        {
            throw new InvalidOperationException("Logger state not initialised");
        }
    }

    private sealed class InitialisedLoggerState : LoggerState
    {
        private readonly string _tableName;
        private readonly DateTime _startDateTime;
        private readonly DateTime _endDateTime;

        public InitialisedLoggerState(string tableName, DateTime startDateTime, DateTime endDateTime)
        {
            _tableName = tableName;
            _startDateTime = startDateTime;
            _endDateTime = endDateTime;
        }

        internal override void Log(ILogger logger, LogLevel logLevel, string format, params object[] args)
        {
            var arguments = new object[] { _tableName, _startDateTime, _endDateTime }
                .Concat(args)
                .ToArray();

            logger.Log(
                logLevel,
                "Compute {TableName} ({StartDateTime:u} - {EndDateTime:u}) " + format,
                arguments);
        }
    }
}