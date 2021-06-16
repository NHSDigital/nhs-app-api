using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.DataPoints;

namespace NHSOnline.Backend.AspNet.Middleware.PerformanceCounter
{
    internal class PerformanceCounterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IStatisticsStoreService _statisticsStoreService;
        private readonly PerformanceCounterConfiguration _options;

        public PerformanceCounterMiddleware(RequestDelegate next,
            IStatisticsStoreService statisticsStoreService,
            PerformanceCounterConfiguration options)
        {
            _next = next;
            _statisticsStoreService = statisticsStoreService;
            _options = options;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (_options.IsMetricLoggingEnabled)
            {
                RegisterRequestCountPerformanceCounter();
                RegisterResponseTimePerformanceCounter(httpContext);
            }

            await _next.Invoke(httpContext);
        }

        private void RegisterRequestCountPerformanceCounter()
        {
            _statisticsStoreService.Add(new RequestCountDataPoint
            {
                UtcTimestampAsUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            });
        }

        private void RegisterResponseTimePerformanceCounter(HttpContext httpContext)
        {
            var watch = new Stopwatch();
            watch.Start();

            httpContext.Response.OnCompleted(() =>
            {
                // Stop the timer
                watch.Stop();

                _statisticsStoreService.Add(new ResponseTimeDataPoint
                {
                    UtcTimestampAsUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    ResponseTimeInMs = watch.ElapsedMilliseconds,
                });

                return Task.CompletedTask;
            });
        }
    }
}