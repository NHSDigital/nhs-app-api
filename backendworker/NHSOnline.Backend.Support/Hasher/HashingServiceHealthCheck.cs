using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support.Hasher
{
    public class HashingServiceHealthCheck: IHealthCheck
    {
        private readonly ILogger _logger;
        private readonly IHashingService _hashingService;

        public HashingServiceHealthCheck(
            ILogger<HashingServiceHealthCheck> logger,
            IHashingService hashingService)
        {
            _logger = logger;
            _hashingService = hashingService;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                return Task.FromResult(
                    !_hashingService.IsDead
                        ? HealthCheckResult.Healthy("Hashing Service is OK")
                        : HealthCheckResult.Degraded("Hashing Service has thrown an error")
                );
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "HashingService health check failed");
                return Task.FromResult(
                    HealthCheckResult.Degraded("HashingService health check failed")
                );
            }
        }
    }
}
