using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Repository;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public class TermsAndConditionsRepository : MongoRepository<IMongoConfiguration, TermsAndConditionsRecord>,
        ITermsAndConditionsRepository
    {
        private readonly ILogger<TermsAndConditionsRepository> _logger;

        public TermsAndConditionsRepository
        (
            ILogger<TermsAndConditionsRepository> logger,
            IApiMongoClient<IMongoConfiguration> mongoClient,
            IMongoConfiguration mongoConfiguration
        ) : base(mongoClient, mongoConfiguration)
        {
            _logger = logger;
        }

        public async Task Create(TermsAndConditionsRecord record)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(record, nameof(record), ThrowError)
                    .IsValid();

                using (_logger.WithTimer("Add terms and conditions consent to Mongo"))
                {
                    await InsertOne(record);
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1309", Justification =
            "Method ‘CompareOrdinal’ is not supported on Mongo Driver")]
        public async Task Update(TermsAndConditionsRecord record)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(record, nameof(record), ThrowError)
                    .IsValid();

                using (_logger.WithTimer("Update terms and conditions consent on Mongo"))
                {
                    await UpdateOne(d => d.NhsLoginId == record.NhsLoginId, record);
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1309", Justification =
            "Method ‘CompareOrdinal’ is not supported on Mongo Driver")]
        public async Task<TermsAndConditionsRecord> Find(string nhsLoginId)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(nhsLoginId, nameof(nhsLoginId), ThrowError)
                    .IsValid();

                using (_logger.WithTimer("Find terms and conditions consent on Mongo"))
                {
                    return await FindOne(d => d.NhsLoginId == nhsLoginId);
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
