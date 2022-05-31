using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Repository.SqlApi
{
    public class SqlApiClientHealthCheck<TConfiguration> : IHealthCheck
        where TConfiguration : ISqlApiRepositoryConfiguration
    {
        private readonly ILogger _logger;
        private readonly ISqlApiClientService _sqlApiClientService;
        private readonly TConfiguration _config;

        public SqlApiClientHealthCheck(
            ILogger<SqlApiClientHealthCheck<TConfiguration>> logger,
            ISqlApiClientService sqlApiClientService,
            TConfiguration config)
        {
            _logger = logger;
            _sqlApiClientService = sqlApiClientService;
            _config = config;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                await _sqlApiClientService.CheckHealthAsync(_config);

                return Healthy();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Cosmos Sql Api Health Check Failed");
                return Failed(exception);
            }
        }

        private static HealthCheckResult Healthy()
            => HealthCheckResult.Healthy($"{typeof(TConfiguration).Name} Cosmos Sql Api Health check passed");

        private static HealthCheckResult Failed(Exception exception)
            => HealthCheckResult.Degraded(
                $"{typeof(TConfiguration).Name} Cosmos Sql Api Health check failed: {exception.Message}",
                exception);
    }
}