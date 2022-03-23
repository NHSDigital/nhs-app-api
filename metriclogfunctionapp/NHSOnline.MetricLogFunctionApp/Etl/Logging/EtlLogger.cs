using System;
using Microsoft.Extensions.Logging;

namespace NHSOnline.MetricLogFunctionApp.Etl.Logging
{
    internal sealed class EtlLogger<TCategoryName> : IEtlLogger<TCategoryName>
    {
        private readonly ILogger _logger;
        private readonly EtlLoggerState _state;

        public EtlLogger(ILogger<TCategoryName> logger, EtlLoggerState state)
        {
            _logger = logger;
            _state = state;
        }

        public void StartedTriggered(string tableName, string context)
        {
            _state.Triggered(tableName, context);

            _state.Log(
                _logger,
                LogLevel.Information,
                "{Action} {Context}",
                "Started",
                context);
        }

        public void Failed(string content = null)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                _state.Log(
                    _logger,
                    LogLevel.Error,
                    "{Action}",
                    nameof(Failed));
            }
            else
            {
                _state.Log(
                    _logger,
                    LogLevel.Error,
                    "{Action} {Content}",
                    nameof(Failed),
                    content);
            }
        }

        public void Information(string content)
        {
            _state.Log(
                _logger,
                LogLevel.Information,
                "{Action} {Content}",
                nameof(Information),
                content);
        }
    }
}