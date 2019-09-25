using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Repository;

namespace NHSOnline.Backend.MessagesApi.Repository
{
    internal class MongoMessageRepository : MongoRepository<UserMessage>, IMessageRepository
    {
        private readonly ILogger<MongoMessageRepository> _logger;
        
        public MongoMessageRepository(
            ILogger<MongoMessageRepository> logger,
            IMongoClient mongoClient,
            IMongoConfiguration mongoConfiguration
        )
            : base(mongoClient, mongoConfiguration)
        {
            _logger = logger;
        }

        public async Task Create(UserMessage userMessage)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(userMessage, nameof(userMessage), ValidateAndLog.ValidationOptions.ThrowError)
                    .IsValid();

                using (_logger.WithTimer("Sending message and nhsLoginId to Mongo"))
                {
                    await InsertOneAsync(userMessage);
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1309",
            Justification = "Method ‘CompareOrdinal’ is not supported on Mongo Driver")]
        public async Task<List<UserMessage>> Find(string nhsLoginId)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(nhsLoginId, nameof(nhsLoginId), ValidateAndLog.ValidationOptions.ThrowError)
                    .IsValid();

                using (_logger.WithTimer("Find messages in Mongo"))
                {
                    var messages= await FindMultipleAsync(d => d.NhsLoginId == nhsLoginId);
                    return messages.ToList();
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

    }
}