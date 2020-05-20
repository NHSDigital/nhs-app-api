using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Repository;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.MessagesApi.Repository
{
    internal class MessageRepository : IMessageRepository
    {
        private readonly ILogger<MessageRepository> _logger;
        private readonly IRepository<UserMessage> _repository;
        private const string RecordName = nameof(UserMessage);

        public MessageRepository
        (   
            ILogger<MessageRepository> logger,
            IRepository<UserMessage> repository
        )
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<RepositoryCreateResult<UserMessage>> Create(UserMessage userMessage)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(userMessage, nameof(userMessage), ThrowError)
                    .IsValid();

               return await _repository.Create(userMessage, RecordName);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1309",
            Justification = "Method 'CompareOrdinal' is not supported in repository")]
        public async Task<RepositoryFindResult<UserMessage>> FindMessagesFromSender(string nhsLoginId, string sender)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(nhsLoginId, nameof(nhsLoginId), ThrowError)
                    .IsNotNull(sender, nameof(sender), ThrowError)
                    .IsValid();

                return await _repository.Find(d => d.NhsLoginId == nhsLoginId && d.Sender == sender, RecordName);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1309",
            Justification = "Method 'CompareOrdinal' is not supported in repository")]
        public async Task<RepositoryFindResult<UserMessage>> FindAllForUser(string nhsLoginId)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(nhsLoginId, nameof(nhsLoginId), ThrowError)
                    .IsValid();

                    return await _repository.Find(x => x.NhsLoginId == nhsLoginId, RecordName);
                
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1309", Justification =
            "Method ‘CompareOrdinal’ is not supported in repository")]
        public async Task<RepositoryFindResult<UserMessage>> FindMessage(string nhsLoginId, string messageId)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(messageId, nameof(messageId), ThrowError)
                    .IsNotNull(nhsLoginId, nameof(nhsLoginId), ThrowError)
                    .IsValid();

                var id = ObjectId.Parse(messageId);

                return await _repository.Find(m => m.Id == id && m.NhsLoginId == nhsLoginId, RecordName);

            }
            finally
            {
                _logger.LogExit();
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1309", Justification =
            "Method ‘CompareOrdinal’ is not supported in repository")]
        public async Task<RepositoryUpdateResult<UserMessage>> UpdateOne(string nhsLoginId, string messageId, UpdateRecordBuilder<UserMessage> updates)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(nhsLoginId, nameof(nhsLoginId), ThrowError)
                    .IsNotNull(messageId, nameof(messageId), ThrowError)
                    .IsValid();

                var id = ObjectId.Parse(messageId);

                return await _repository.Update(m => m.Id == id && m.NhsLoginId == nhsLoginId,
                        updates,
                        RecordName);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}