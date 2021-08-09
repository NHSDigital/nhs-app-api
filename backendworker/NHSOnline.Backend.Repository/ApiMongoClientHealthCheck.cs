using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Repository
{
    internal sealed class ApiMongoClientHealthCheck<TConfiguration>: IHealthCheck
        where TConfiguration : IRepositoryConfiguration
    {
        private readonly ILogger _logger;
        private readonly IMongoClientService _mongoClientService;
        private readonly TConfiguration _configuration;

        public ApiMongoClientHealthCheck(
            ILogger<ApiMongoClientHealthCheck<TConfiguration>> logger,
            IMongoClientService mongoClientService,
            TConfiguration configuration)
        {
            _logger = logger;
            _mongoClientService = mongoClientService;
            _configuration = configuration;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                await _mongoClientService.CheckHealthAsync(_configuration.DatabaseName);

                return Healthy();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Mongo Health Check Failed");
                return Failed(exception);
            }
        }

        private static HealthCheckResult Healthy()
            => HealthCheckResult.Healthy($"{typeof(TConfiguration).Name} Mongo Health check passed");

        private static HealthCheckResult Failed(Exception exception)
            => HealthCheckResult.Degraded(
                $"{typeof(TConfiguration).Name} Mongo Health check failed: {exception.Message}",
                exception);
    }
}