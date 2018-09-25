using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Support
{
    public sealed class PerformanceTimer : IDisposable
    {
        private readonly string _name;
        private readonly ILogger _logger;
        private readonly Stopwatch _stopwatch;

        internal PerformanceTimer(ILogger logger, string name)
        {
            _name = name;
            _logger = logger;

            _logger.LogDebug($"Performance, Starting '{_name}'");
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _logger.LogDebug($"Performance, Finished '{_name}'");
            
            var milliseconds = _stopwatch.ElapsedMilliseconds;
            _logger.LogInformation($"Performance, '{_name}' completed in {milliseconds}ms");
        }
    }

    public static class PerformanceTimerExtensions
    {
        public static IDisposable WithTimer(this ILogger logger, string name)
        {
            return new PerformanceTimer(logger, name);
        }
    }
}