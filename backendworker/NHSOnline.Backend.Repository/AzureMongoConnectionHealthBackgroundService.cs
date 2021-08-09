using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Repository
{
    public class AzureMongoConnectionHealthBackgroundService : BackgroundService
    {
        private readonly IMongoClientService _mongoClientService;
        private readonly ILogger<AzureMongoConnectionHealthBackgroundService> _logger;
        private readonly int _mongoDbConnectionCheckIntervalInSeconds;
        private readonly string _mongoDatabaseName;

        private static readonly Guid Identifier = Guid.NewGuid();
        private static uint _counter;

        public AzureMongoConnectionHealthBackgroundService(
            IMongoClientService mongoClientService,
            ILogger<AzureMongoConnectionHealthBackgroundService> logger,
            IConfiguration configuration)
        {
            _mongoClientService = mongoClientService;
            _logger = logger;
            _mongoDbConnectionCheckIntervalInSeconds = configuration.GetIntOrThrow("MONGO_DB_CONNECTION_CHECK_INTERVAL_IN_SECONDS", _logger);
            _mongoDatabaseName = configuration.GetOrThrow("MONGO_DATABASE_NAME", _logger);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Starting {nameof(AzureMongoConnectionHealthBackgroundService)}");
            if (_mongoClientService.SupportsConnectionRecovery)
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (_counter == uint.MaxValue) { _counter = uint.MinValue; }
                    await _mongoClientService.RebuildIfNecessary(_mongoDatabaseName, Identifier, _counter++);
                    await Task.Delay(_mongoDbConnectionCheckIntervalInSeconds * 1000, stoppingToken);
                }
            }
            _logger.LogInformation($"Stopping {nameof(AzureMongoConnectionHealthBackgroundService)}");
        }
    }
}
