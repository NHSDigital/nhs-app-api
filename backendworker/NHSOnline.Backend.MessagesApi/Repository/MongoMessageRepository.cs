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
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.MessagesApi.Repository
{
    internal class MongoMessageRepository : MongoRepository<IMongoConfiguration, UserMessage>, IMessageRepository
    {
        private readonly ILogger<MongoMessageRepository> _logger;

        public MongoMessageRepository
        (
            ILogger<MongoMessageRepository> logger,
            IApiMongoClient<IMongoConfiguration> mongoClient,
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
                    .IsNotNull(userMessage, nameof(userMessage), ThrowError)
                    .IsValid();

                using (_logger.WithTimer("Sending message and nhsLoginId to Mongo"))
                {
                    await InsertOne(userMessage);
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1309",
            Justification = "Method 'CompareOrdinal' is not supported on Mongo Driver")]
        public async Task<List<UserMessage>> Find(string nhsLoginId, string sender)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(nhsLoginId, nameof(nhsLoginId), ThrowError)
                    .IsNotNull(sender, nameof(sender), ThrowError)
                    .IsValid();

                using (_logger.WithTimer("Find messages in Mongo"))
                {
                    var messages = await Find(d => d.NhsLoginId == nhsLoginId && d.Sender == sender);
                    return messages.ToList();
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1309",
            Justification = "Method 'CompareOrdinal' is not supported on Mongo Driver")]
        public async Task<List<SummaryMessage>> Summary(string nhsLoginId)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(nhsLoginId, nameof(nhsLoginId), ThrowError)
                    .IsValid();

                using (_logger.WithTimer("Aggregate summary messages in Mongo"))
                {
                    return await Aggregate()
                        .Match(x => x.NhsLoginId == nhsLoginId)
                        .SortByDescending(x => x.SentTime)
                        .Group(
                            k => new { k.NhsLoginId, k.Sender },
                            g => new SummaryMessage
                            {
                                UnreadCount = g.Count(v => !v.Read.HasValue),
                                Id = g.First().Id,
                                NhsLoginId = g.First().NhsLoginId,
                                Sender = g.First().Sender,
                                Version = g.First().Version,
                                Body = g.First().Body,
                                Read = g.First().Read,
                                SentTime = g.First().SentTime
                            }
                        )
                        .ToListAsync();
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}