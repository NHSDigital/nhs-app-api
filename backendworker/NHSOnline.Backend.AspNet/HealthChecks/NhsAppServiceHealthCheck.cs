using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.AspNet.CorrelationId;

namespace NHSOnline.Backend.AspNet.HealthChecks
{
    internal static class NhsAppServiceHealthCheck
    {
        internal const string ReadinessPath = "/health/ready";
    }

    internal sealed class NhsAppServiceHealthCheck<TNhsAppHealthCheckClient>: IHealthCheck
        where TNhsAppHealthCheckClient: INhsAppHealthCheckClient
    {
        private readonly ILogger _logger;
        private readonly INhsAppHealthCheckClient _client;

        public NhsAppServiceHealthCheck(
            ILogger<NhsAppServiceHealthCheck<TNhsAppHealthCheckClient>> logger,
            TNhsAppHealthCheckClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                context.PrepareCorrelationIdContext();

                using var request = new HttpRequestMessage(HttpMethod.Get, NhsAppServiceHealthCheck.ReadinessPath);

                using var response = await _client.Client.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

                return response.StatusCode switch
                {
                    HttpStatusCode.NoContent => Healthy(response.StatusCode),
                    HttpStatusCode.ServiceUnavailable => Degraded(response.StatusCode),
                    _ => Failed(response.StatusCode)
                };
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "NHS App service health check failed");
                return Failed(exception);
            }
        }

        private static HealthCheckResult Healthy(HttpStatusCode httpStatusCode)
            => HealthCheckResult.Healthy($"{typeof(TNhsAppHealthCheckClient).Name} Health check passed: {httpStatusCode}");

        private static HealthCheckResult Degraded(HttpStatusCode httpStatusCode)
            => HealthCheckResult.Degraded($"{typeof(TNhsAppHealthCheckClient).Name} Health check degraded: {httpStatusCode}");

        private static HealthCheckResult Failed(HttpStatusCode httpStatusCode)
            => HealthCheckResult.Degraded($"{typeof(TNhsAppHealthCheckClient).Name} Health check failed: {httpStatusCode}");

        private static HealthCheckResult Failed(Exception exception)
            => HealthCheckResult.Degraded($"{typeof(TNhsAppHealthCheckClient).Name} Health check failed", exception);
    }
}