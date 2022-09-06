using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Repository.SqlApi;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.Messages.Repository
{
    public class SenderRepository : ISenderRepository
    {
        private readonly ILogger<SenderRepository> _logger;
        private readonly ISqlApiRepository<DbSender> _repository;
        private const string RecordName = nameof(Sender);

        public SenderRepository(
            ILogger<SenderRepository> logger,
            ISqlApiRepository<DbSender> repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<RepositoryCreateResult<DbSender>> CreateOrUpdate(DbSender sender)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(sender, nameof(sender), ThrowError)
                    .IsValid();

                return await _repository.CreateOrUpdate(
                    sender,
                    RecordName,
                    sender.Id);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<RepositoryFindResult<DbSender>> Find(string senderId)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(senderId, nameof(senderId), ThrowError)
                    .IsValid();

                return await _repository.Find(senderId, senderId, RecordName);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<RepositoryFindResult<DbSender>> Find(DateTime lastUpdatedBefore, int limit)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(lastUpdatedBefore, nameof(lastUpdatedBefore), ThrowError)
                    .IsNotNull(limit, nameof(limit), ThrowError)
                    .IsValid();

                IQueryable<DbSender> QueryFunction(IQueryable<DbSender> query) =>
                    query.Where(x => x.Timestamp <= lastUpdatedBefore)
                        .OrderBy(x => x.Timestamp)
                        .Take(limit);

                return await _repository.Find(QueryFunction, RecordName);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}