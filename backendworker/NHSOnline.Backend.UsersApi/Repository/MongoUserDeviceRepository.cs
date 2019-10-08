using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Repository;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.UsersApi.Repository
{
    internal class MongoUserDeviceRepository : MongoRepository<IMongoConfiguration, UserDevice>, IUserDeviceRepository
    {
        private readonly ILogger<MongoUserDeviceRepository> _logger;

        public MongoUserDeviceRepository
        (
            ILogger<MongoUserDeviceRepository> logger,
            IApiMongoClient<IMongoConfiguration> mongoClient,
            IMongoConfiguration mongoConfiguration
        )
            : base(mongoClient, mongoConfiguration)
        {
            _logger = logger;
        }

        public async Task Create(UserDevice userDevice)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(userDevice, nameof(userDevice), ThrowError)
                    .IsValid();

                using (_logger.WithTimer("Add user device to Mongo"))
                {
                    await InsertOne(userDevice);
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1309", Justification =
            "Method ‘CompareOrdinal’ is not supported on Mongo Driver")]
        public async Task<UserDevice> Find(string nhsLoginId, string deviceId)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(deviceId, nameof(deviceId), ThrowError)
                    .IsNotNull(nhsLoginId, nameof(nhsLoginId), ThrowError)
                    .IsValid();

                using (_logger.WithTimer("Find user device on Mongo"))
                {
                    return await FindOne(d => d.NhsLoginId == nhsLoginId && d.DeviceId == deviceId);
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1309", Justification =
            "Method ‘CompareOrdinal’ is not supported on Mongo Driver")]
        public async Task Delete(string nhsLoginId, string deviceId)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(deviceId, nameof(deviceId), ThrowError)
                    .IsNotNull(nhsLoginId, nameof(nhsLoginId), ThrowError)
                    .IsValid();

                using (_logger.WithTimer("Delete user device on Mongo"))
                {
                    await DeleteOne(d => d.NhsLoginId == nhsLoginId && d.DeviceId == deviceId);
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}