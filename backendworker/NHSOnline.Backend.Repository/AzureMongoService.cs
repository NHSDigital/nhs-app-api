using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using NHSOnline.Backend.Support.AzureManagement;

namespace NHSOnline.Backend.Repository
{
    public class AzureMongoService : IMongoClientService, IDisposable
    {
        private readonly IAzureMongoClient _azureMongoClient;
        private readonly IMongoClientCreator _mongoClientCreator;
        private readonly IAzureKeyVaultService _azureKeyVaultService;
        private readonly ILogger<AzureMongoService> _logger;
        private readonly Semaphore _buildClientsSemaphore;

        private const string ClientTypeKeyName = "clientType";
        private static readonly Guid Identifier = Guid.NewGuid();

        public AzureMongoService(
            ILogger<AzureMongoService> logger,
            IMongoClientCreator mongoClientCreator,
            IAzureKeyVaultService azureKeyVaultService,
            IAzureMongoClient azureMongoClient)
        {
            _logger = logger;
            _mongoClientCreator = mongoClientCreator;
            _azureKeyVaultService = azureKeyVaultService;
            _buildClientsSemaphore = new Semaphore(1, maximumCount: 1);
            _azureMongoClient = azureMongoClient;

            BuildClients().GetAwaiter().GetResult();
        }

        public async Task RebuildIfNecessary(string databaseName, Guid identifier, uint counter = 0)
        {
            try
            {
                _buildClientsSemaphore.WaitOne();
                await ConnectionRecoveryIfNeeded(databaseName, identifier, counter);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while recovering mongo connections");
            }
            finally
            {
                _buildClientsSemaphore.Release();
            }
        }

        public bool SupportsConnectionRecovery => true;

        public async Task InsertOneAsync<TRecord>(IRepositoryConfiguration config, TRecord record) where TRecord : RepositoryRecord
        {
            await DoWithRecoveryAsync(
                config.DatabaseName,
                async mongoDatabase => await mongoDatabase
                    .GetCollection<TRecord>(config.CollectionName)
                    .InsertOneAsync(record));
        }

        public async Task<ReplaceOneResult> ReplaceOneAsync<TRecord>(IRepositoryConfiguration config, Expression<Func<TRecord, bool>> filter, TRecord record, ReplaceOptions replaceOptions) where TRecord : RepositoryRecord
        {
            return await DoWithRecoveryAsync(
                config.DatabaseName,
                async mongoDatabase => await mongoDatabase
                    .GetCollection<TRecord>(config.CollectionName)
                    .ReplaceOneAsync(filter, record, replaceOptions));
        }

        public async Task<DeleteResult> DeleteOneAsync<TRecord>(IRepositoryConfiguration config, Expression<Func<TRecord, bool>> filter) where TRecord : RepositoryRecord
        {
            return await DoWithRecoveryAsync(
                config.DatabaseName,
                async mongoDatabase => await mongoDatabase
                    .GetCollection<TRecord>(config.CollectionName)
                    .DeleteOneAsync(filter));
        }

        public async Task<IAsyncCursor<TRecord>> FindAsync<TRecord>(IRepositoryConfiguration config, Expression<Func<TRecord, bool>> filter, FindOptions<TRecord> findOptions) where TRecord : RepositoryRecord
        {
            return await DoWithRecoveryAsync(
                config.DatabaseName,
                async mongoDatabase => await mongoDatabase
                    .GetCollection<TRecord>(config.CollectionName)
                    .FindAsync(filter, findOptions));
        }

        public async Task<long> CountAsync<TRecord>(IRepositoryConfiguration config, Expression<Func<TRecord, bool>> filter) where TRecord : RepositoryRecord
        {
            return await DoWithRecoveryAsync(
                config.DatabaseName,
                async mongoDatabase => await mongoDatabase
                    .GetCollection<TRecord>(config.CollectionName)
                    .CountDocumentsAsync(filter));
        }

        public async Task<UpdateResult> UpdateManyAsync<TRecord>(IRepositoryConfiguration config, Expression<Func<TRecord, bool>> filter, UpdateDefinition<TRecord> updates) where TRecord : RepositoryRecord
        {
            return await DoWithRecoveryAsync(
                config.DatabaseName,
                async mongoDatabase => await mongoDatabase
                    .GetCollection<TRecord>(config.CollectionName)
                    .UpdateManyAsync(filter, updates));
        }

        public async Task InsertOneDocumentAsync(IRepositoryConfiguration config, BsonDocument record)
        {
            await DoWithRecoveryAsync(
                config.DatabaseName,
                async mongoDatabase => await mongoDatabase
                    .GetCollection<BsonDocument>(config.CollectionName)
                    .InsertOneAsync(record));
        }

        public async Task<UpdateResult> UpdateOneDocumentAsync(IRepositoryConfiguration config, FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update)
        {
            return await DoWithRecoveryAsync(
                config.DatabaseName,
                async mongoDatabase => await mongoDatabase
                    .GetCollection<BsonDocument>(config.CollectionName)
                    .UpdateOneAsync(filter, update));
        }

        public async Task<BsonDocument> FindOneAndUpdateDocumentAsync(IRepositoryConfiguration config, FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update)
        {
            return await DoWithRecoveryAsync(
                config.DatabaseName,
                async mongoDatabase => await mongoDatabase
                    .GetCollection<BsonDocument>(config.CollectionName)
                    .FindOneAndUpdateAsync(filter, update));
        }

        public async Task<BsonDocument> FindFirstDocument(IRepositoryConfiguration config, FilterDefinition<BsonDocument> filter)
        {
            return await DoWithRecoveryAsync(
                config.DatabaseName,
                async mongoDatabase => await mongoDatabase
                    .GetCollection<BsonDocument>(config.CollectionName)
                    .Find(filter).FirstOrDefaultAsync());
        }

        public async Task<DeleteResult> DeleteOneDocumentAsync(IRepositoryConfiguration config, FilterDefinition<BsonDocument> filter)
        {
            return await DoWithRecoveryAsync(
                config.DatabaseName,
                async mongoDatabase => await mongoDatabase
                    .GetCollection<BsonDocument>(config.CollectionName)
                    .DeleteOneAsync(filter));
        }

        public async Task CheckHealthAsync(string databaseName)
        {
            await DoWithRecoveryAsync(databaseName, async mongoDatabase =>
            {
                await CheckHealthCoreAsync(mongoDatabase);
            });
        }

        private async Task ConnectionRecoveryIfNeeded(string databaseName, Guid identifier, uint counter)
        {
            if (!_azureMongoClient.UsingPrimary || !await CheckHealthAsync(_azureMongoClient.PrimaryClient, databaseName))
            {
                _logger.LogInformation($"Rebuilding primary client, identifier {identifier}, count {counter}");
                var connectionStrings = await _azureKeyVaultService.GetConnectionStrings();
                if (connectionStrings != null)
                {
                    _azureMongoClient.PrimaryClient = _mongoClientCreator.CreatePrimaryMongoClient(connectionStrings.PrimaryConnectionString);
                    _logger.LogInformation($"Rebuilt primary client, identifier {identifier}, count {counter}");
                    _azureMongoClient.UsingPrimary = await CheckHealthAsync(_azureMongoClient.PrimaryClient, databaseName);
                }
                else
                {
                    _logger.LogInformation("Could not rebuild primary client - connection string was null");
                }
            }

            var secondaryHealthy = await CheckHealthAsync(_azureMongoClient.SecondaryClient, databaseName);
            if (!secondaryHealthy)
            {
                _logger.LogInformation($"Rebuilding secondary client, identifier {identifier}, count {counter}");
                var connectionStrings = await _azureKeyVaultService.GetConnectionStrings();
                if (connectionStrings != null)
                {
                    _azureMongoClient.SecondaryClient = _mongoClientCreator.CreateSecondaryMongoClient(connectionStrings.SecondaryConnectionString);
                    _logger.LogInformation($"Rebuilt secondary client, identifier {identifier}, count {counter}");
                    secondaryHealthy = await CheckHealthAsync(_azureMongoClient.SecondaryClient, databaseName);
                }
                else
                {
                    _logger.LogInformation("Could not rebuild secondary client - connection string was null");
                }
            }

            _azureMongoClient.IsHealthy = _azureMongoClient.UsingPrimary || secondaryHealthy;

            _logger.LogInformation(
                $"Connection Health: identifier {identifier}, count {counter}, primary:{_azureMongoClient.UsingPrimary} secondary:{secondaryHealthy} activeClient:{(_azureMongoClient.IsHealthy ? (_azureMongoClient.UsingPrimary ? "primary" : "secondary") : "none")}");
        }

        private async Task DoWithRecoveryAsync(string databaseName, Func<IMongoDatabase, Task> crudAction)
        {
            await DoWithRecoveryAsync<object>(databaseName, async mongoDatabase =>
            {
                await crudAction(mongoDatabase);
                return null;
            });
        }

        private async Task<TResult> DoWithRecoveryAsync<TResult>(string databaseName, Func<IMongoDatabase, Task<TResult>> crudAction)
        {
            try
            {
                var (result, _) = await DoAction(databaseName, crudAction);
                return result;
            }
            catch (Exception e) when (
                e is MongoAuthenticationException ||
                e is MongoCommandException { Code: 13 } ||
                e is MongoWriteException { WriteError: { Code: 13 } }) // See MongoDB.Driver.Core/ServerErrorCode.cs
            {
                if (e.Data[ClientTypeKeyName] != null)
                {
                    var initialClientType = (AzureMongoClientType) e.Data[ClientTypeKeyName];

                    _logger.LogInformation($"{nameof(DoWithRecoveryAsync)} - Authentication error talking to " +
                                           $"database with {initialClientType} client");

                    _azureMongoClient.ReportAuthenticationFailure(initialClientType);
                }
                else
                {
                    _logger.LogInformation($"{nameof(DoWithRecoveryAsync)} - Authentication error talking to " +
                                           $"database - Unable to determine client");
                }

                var (retryResult, newClientType) = await DoAction(databaseName, crudAction);

                _logger.LogInformation($"{nameof(DoWithRecoveryAsync)} - Successful action after recovery " +
                                       $"with {newClientType} client");

                return retryResult;
            }
        }

        private async Task<Tuple<TResult, AzureMongoClientType>> DoAction<TResult>(string databaseName, Func<IMongoDatabase, Task<TResult>> crudAction)
        {
            var clientType = AzureMongoClientType.Unknown;
            try {
                var mongoDatabaseContainer = await GetDatabase(databaseName);
                clientType = mongoDatabaseContainer.ClientType;
                return Tuple.Create(await crudAction(mongoDatabaseContainer.MongoDatabase), clientType);
            }
            catch (Exception ex)
            {
                ex.Data.Add(ClientTypeKeyName, clientType);
                _logger.LogError(ex, $"{nameof(DoWithRecoveryAsync)} - Unsuccessful action with {clientType} client");

                throw;
            }
        }

        private async Task<MongoDatabaseContainer> GetDatabase(string databaseName)
        {
            var activeMongoClient = await GetActiveMongoClient(databaseName);
            var database = activeMongoClient.GetDatabase(databaseName);

            return new MongoDatabaseContainer
            {
                MongoDatabase = database,
                ClientType = activeMongoClient.ClientType
            };
        }

        private async Task<INamedMongoClient> GetActiveMongoClient(string databaseName)
        {
            if (!_azureMongoClient.IsHealthy)
            {
                await RebuildIfNecessary(databaseName, Identifier);
            }

            return _azureMongoClient.ActiveClient;
        }

        private async Task BuildClients()
        {
            _logger.LogInformation("Fetching connection strings from Azure KeyVault");
            var connectionStrings = await _azureKeyVaultService.GetConnectionStrings();

            if (connectionStrings != null)
            {
                _azureMongoClient.Initialize(
                    _mongoClientCreator.CreatePrimaryMongoClient(connectionStrings.PrimaryConnectionString),
                    _mongoClientCreator.CreateSecondaryMongoClient(connectionStrings.SecondaryConnectionString)
                );
                _logger.LogInformation("Primary and secondary clients created");
            }
            else
            {
                _logger.LogInformation("Could not create primary and secondary clients - connection strings were null");
            }
        }

        private async Task<bool> CheckHealthAsync(IMongoClient mongoClient, string databaseName, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                await CheckHealthCoreAsync(mongoClient.GetDatabase(databaseName), cancellationToken);

                return true;
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Error doing health check - {e.Message}");
                return false;
            }
        }

        private static async Task CheckHealthCoreAsync(IMongoDatabase mongoDatabase, CancellationToken cancellationToken = new CancellationToken())
        {
            using var cursor = await mongoDatabase
                .ListCollectionNamesAsync(cancellationToken: cancellationToken);
            await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public void Dispose()
        {
            _buildClientsSemaphore?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
