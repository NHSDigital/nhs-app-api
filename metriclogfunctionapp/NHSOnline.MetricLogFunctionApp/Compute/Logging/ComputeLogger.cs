using System;
using Microsoft.Extensions.Logging;

namespace NHSOnline.MetricLogFunctionApp.Compute.Logging
{
    internal sealed class ComputeLogger<TCategoryName>: IComputeLogger<TCategoryName>
    {
        private readonly ILogger _logger;
        private readonly ComputeLoggerState _state;

        public ComputeLogger(ILogger<TCategoryName> logger, ComputeLoggerState state)
        {
            _logger = logger;
            _state = state;
        }

        public void Started(string tableName, DateTime startDateTime, DateTime endDateTime)
        {
            _state.Started(tableName, startDateTime, endDateTime);

            _state.Log(
                _logger,
                LogLevel.Information,
                "{Action}",
                "Started");
        }

        public void DuplicatedDataFound()
        {
            _state.Log(
                _logger,
                LogLevel.Error,
                "{Action}",
                nameof(DuplicatedDataFound));
        }

        public void Failed()
        {
            _state.Log(
                _logger,
                LogLevel.Error,
                "{Action}",
                nameof(Failed));
        }
    }
}