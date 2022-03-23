using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace NHSOnline.MetricLogFunctionApp.Etl.Logging
{
    internal class EtlLoggerState
    {
        private EtlPeriodState _periodState = new NullPeriodState();

        internal void Triggered(string tableName, string context)
            => _periodState = new EtlTriggeredSnapshotLoggerState(tableName, context);

        internal void Log(ILogger logger, LogLevel logLevel, string format, params object[] args)
            => _periodState.Log(logger, logLevel, format, args);

        private abstract class EtlPeriodState
        {
            internal abstract void Log(ILogger logger, LogLevel logLevel, string format, params object[] args);
        }

        private sealed class NullPeriodState : EtlPeriodState
        {
            internal override void Log(ILogger logger, LogLevel logLevel, string format, params object[] args)
            {
                throw new InvalidOperationException("Period state not initialised");
            }
        }

        private sealed class EtlTriggeredSnapshotLoggerState : EtlPeriodState
        {
            private readonly string _tableName;
            private readonly string _context;

            public EtlTriggeredSnapshotLoggerState(string tableName, string context)
            {
                _tableName = tableName;
                _context = context;
            }

            internal override void Log(ILogger logger, LogLevel logLevel, string format, params object[] args)
            {
                var arguments = new object[] { "Triggered", _tableName, _context }
                    .Concat(args)
                    .ToArray();

                logger.Log(
                    logLevel,
                    "ETL {Period} {TableName} ({Context}) " + format,
                    arguments);
            }
        }
    }
}