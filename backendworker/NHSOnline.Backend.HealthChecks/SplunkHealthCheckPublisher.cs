using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.HealthChecks
{
    internal sealed class SplunkHealthCheckPublisher: IHealthCheckPublisher
    {
        private readonly ILogger _logger;

        public SplunkHealthCheckPublisher(ILogger<SplunkHealthCheckPublisher> logger)
        {
            _logger = logger;
        }

        public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            PublishReportStatus(report);

            if (report.Status != HealthStatus.Healthy)
            {
                PublishReportEntries(report);
            }

            return Task.CompletedTask;
        }

        private void PublishReportStatus(HealthReport report)
        {
            _logger.Log(DetermineLogLevel(report.Status), "HealthStatus={HealthStatus}", report.Status);
        }

        private void PublishReportEntries(HealthReport report)
        {
            foreach (var (reportName, reportEntry) in report.Entries)
            {
                _logger.Log(
                    DetermineLogLevel(reportEntry.Status),
                    reportEntry.Exception,
                    "HealthCheck={HealthCheck} HealthCheckStatus={HealthCheckStatus} {HealthCheckDescription}",
                    reportName,
                    reportEntry.Status,
                    reportEntry.Description);
            }
        }

        private static LogLevel DetermineLogLevel(HealthStatus status)
            => status switch
            {
                HealthStatus.Healthy => LogLevel.Information,
                _ => LogLevel.Warning
            };
    }
}