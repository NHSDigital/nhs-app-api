using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.AspNet.HealthChecks
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
            switch (report.Status)
            {
                case HealthStatus.Healthy:
                    PublishHealthyReport(report);
                    break;

                default:
                    PublishNotHealthyReport(report);
                    break;
            }

            return Task.CompletedTask;
        }

        private void PublishHealthyReport(HealthReport report)
        {
            _logger.Log(
                DetermineLogLevel(report.Status),
                "HealthStatus={HealthStatus} HealthChecks={HealthChecks}",
                report.Status,
                string.Join(";", report.Entries.Keys));
        }

        private void PublishNotHealthyReport(HealthReport report)
        {
            _logger.Log(DetermineLogLevel(report.Status), "HealthStatus={HealthStatus}", report.Status);
            PublishReportEntries(report);
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