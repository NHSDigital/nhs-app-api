using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using NHSOnline.Backend.Messages.Areas.Messages;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Repository;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.Messages.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ILogger<MessageRepository> _logger;
        private readonly IRepository<UserMessage> _repository;
        private readonly ICanonicalSenderNameService _canonicalSenderNameService;
        private const string RecordName = nameof(UserMessage);

        public MessageRepository
        (
            ILogger<MessageRepository> logger,
            IRepository<UserMessage> repository,
            ICanonicalSenderNameService canonicalSenderNameService
        )
        {
            _logger = logger;
            _repository = repository;
            _canonicalSenderNameService = canonicalSenderNameService;
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
        public async Task<RepositoryFindResult<UserMessage>> FindMessagesFromSenderByName(string nhsLoginId, string sender)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(nhsLoginId, nameof(nhsLoginId), ThrowError)
                    .IsNotNull(sender, nameof(sender), ThrowError)
                    .IsValid();

                return await Find(d => d.NhsLoginId == nhsLoginId && d.Sender == sender);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1309",
            Justification = "Method 'CompareOrdinal' is not supported in repository")]
        public async Task<RepositoryFindResult<UserMessage>> FindAllForUserV1(string nhsLoginId)
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

        [SuppressMessage("Microsoft.Globalization", "CA1309",
            Justification = "Method 'CompareOrdinal' is not supported in repository")]
        public async Task<RepositoryFindResult<UserMessage>> FindMessagesFromSenderById(string nhsLoginId, string senderId)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(nhsLoginId, nameof(nhsLoginId), ThrowError)
                    .IsNotNull(senderId, nameof(senderId), ThrowError)
                    .IsValid();

                return await FindWithPostQueryFilter(d => d.NhsLoginId == nhsLoginId,
                    d => d.SenderContext.SenderId == senderId);
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

                    return await Find(x => x.NhsLoginId == nhsLoginId);
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

                return await Find(m => m.Id == id && m.NhsLoginId == nhsLoginId);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1309", Justification =
            "Method ‘CompareOrdinal’ is not supported in repository")]
        public async Task<RepositoryUpdateResult<UserMessage>> UpdateOne(string nhsLoginId, string messageId,
            (List<Expression<Func<UserMessage, bool>>> filters, UpdateRecordBuilder<UserMessage> updates) filtersAndUpdates)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(nhsLoginId, nameof(nhsLoginId), ThrowError)
                    .IsNotNull(messageId, nameof(messageId), ThrowError)
                    .IsValid();

                var id = ObjectId.Parse(messageId);

                Expression<Func<UserMessage, bool>> userMessageFilter = userMessage =>
                    userMessage.Id == id && userMessage.NhsLoginId == nhsLoginId;

                var (filters, updates) = filtersAndUpdates;
                foreach (var filter in filters)
                {
                    // We need to traverse the filter expression and replace the UserMessage instance used in the body
                    // with the instance passed into the userMessageFilter expression.  This step is required so we
                    // can combine the expressions into a single expression
                    //
                    // Example:
                    // Combining expressions:
                    //          a => a.Id == "abc" and b => b.NhsLoginId == "def"
                    // results in combined expression:
                    //          a => a.Id == "abc" && b.NhsLoginId == "def"
                    // We must modify this expression to remove the reference to instance b:
                    //          a => a.Id == "abc" && a.NhsLoginId == "def"
                    // See stackoverflow https://stackoverflow.com/a/71721612

                    var filterUserMessageInstance = filter.Parameters[0];
                    var exprUserMessageInstance = userMessageFilter.Parameters[0];
                    var filterBody = ExpressionModifier.Replace(
                        filter.Body, filterUserMessageInstance, exprUserMessageInstance);

                    var combinedExpression = Expression.AndAlso(
                        userMessageFilter.Body,
                        filterBody);
                    userMessageFilter = Expression.Lambda<Func<UserMessage,bool>>(combinedExpression, userMessageFilter.Parameters[0]);
                }

                return await _repository.Update(userMessageFilter, updates, RecordName);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<RepositoryFindResult<UserMessage>> Find(
            Expression<Func<UserMessage, bool>> filter,
            int? maxRecords = null)
        {
            var result = await _repository.Find(filter, RecordName, maxRecords);

            if (result is RepositoryFindResult<UserMessage>.Found foundResult)
            {
                await _canonicalSenderNameService.UpdateWithCanonicalSenderName(foundResult.Records);
            }

            return result;
        }

        private async Task<RepositoryFindResult<UserMessage>> FindWithPostQueryFilter(
            Expression<Func<UserMessage, bool>> filter,
            Func<UserMessage, bool> postQueryFilter,
            int? maxRecords = null)
        {
            var result = await _repository.Find(filter, RecordName, maxRecords);

            if (result is RepositoryFindResult<UserMessage>.Found foundResult)
            {
                await _canonicalSenderNameService.UpdateWithCanonicalSenderName(foundResult.Records);

                foreach (var record in foundResult.Records.Where(r => !postQueryFilter(r)).ToList())
                {
                    foundResult.Records.Remove(record);
                }
            }

            return result;
        }
    }
}
