using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Repository;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public class TermsAndConditionsRepository : ITermsAndConditionsRepository
    {
        private readonly ILogger<TermsAndConditionsRepository> _logger;
        private readonly IRepository<TermsAndConditionsRecord> _repository;
        private const string RecordName = nameof(TermsAndConditionsRecord);

        public TermsAndConditionsRepository
        (
            ILogger<TermsAndConditionsRepository> logger,
            IRepository<TermsAndConditionsRecord> repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<RepositoryCreateResult<TermsAndConditionsRecord>> Create(TermsAndConditionsRecord record)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(record, nameof(record), ThrowError)
                    .IsValid();

                return await _repository.Create(record, RecordName);

            }
            finally
            {
                _logger.LogExit();
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1309", Justification =
            "Method ‘CompareOrdinal’ is not supported in repository")]
        public async Task<RepositoryUpdateResult<TermsAndConditionsRecord>> Update(string nhsLoginId,
            UpdateRecordBuilder<TermsAndConditionsRecord> updates)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(nhsLoginId, nameof(nhsLoginId), ThrowError)
                    .IsNotNull(updates, nameof(updates), ThrowError)
                    .IsValid();

                return await _repository.Update(
                    d => d.NhsLoginId == nhsLoginId,
                    updates,
                    RecordName);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1309", Justification =
            "Method ‘CompareOrdinal’ is not supported in repository")]
        public async Task<RepositoryFindResult<TermsAndConditionsRecord>> Find(string nhsLoginId)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(nhsLoginId, nameof(nhsLoginId), ThrowError)
                    .IsValid();

                return await _repository.Find(d => d.NhsLoginId == nhsLoginId,
                    RecordName);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}