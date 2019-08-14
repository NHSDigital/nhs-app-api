using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.UsersApi.Repository
{
    internal class MongoUserDeviceRepository : MongoRepository<UserDevice>, IUserDeviceRepository
    {
        protected override string DatabaseName { get; }
        protected override string CollectionName { get; }

        private readonly ILogger<MongoUserDeviceRepository> _logger;
        
        public MongoUserDeviceRepository(
            ILogger<MongoUserDeviceRepository> logger,
            IMongoClient mongoClient,
            IMongoConfiguration config
        )
            : base(mongoClient)
        {
            _logger = logger;

            DatabaseName = config.DatabaseName;
            CollectionName = config.UserDeviceCollectionName;
        }

        public async Task Create(UserDevice userDevice)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(userDevice, nameof(userDevice), ValidateAndLog.ValidationOptions.ThrowError)
                    .IsValid();

                using (_logger.WithTimer("Add user device to Mongo"))
                {
                    await InsertOneAsync(userDevice);
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}