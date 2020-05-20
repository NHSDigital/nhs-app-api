using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.UserInfoApi.Repository
{
    [SuppressMessage("Microsoft.Globalization", "CA1309",
        Justification = "Method �CompareOrdinal� is not supported in repository")]
    internal class UserInfoRepository : IInfoRepository
    {
        private readonly ILogger<UserInfoRepository> _logger;
        private readonly IRepository<UserAndInfo> _repository;
        private const string RecordName = nameof(UserAndInfo);

        public UserInfoRepository(
            ILogger<UserInfoRepository> logger,
            IRepository<UserAndInfo> repository
        )
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<RepositoryCreateResult<UserAndInfo>> Create(UserAndInfo userAndInfo)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(userAndInfo, nameof(userAndInfo), ValidateAndLog.ValidationOptions.ThrowError)
                    .IsValid();

                return await _repository.CreateOrUpdate(d => d.NhsLoginId == userAndInfo.NhsLoginId,
                    userAndInfo,
                    RecordName);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<RepositoryFindResult<UserAndInfo>> FindByNhsLoginId(string nhsLoginId)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(nhsLoginId, nameof(nhsLoginId), ValidateAndLog.ValidationOptions.ThrowError)
                    .IsValid();

                return await _repository.Find(d => d.NhsLoginId == nhsLoginId, RecordName);

            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<RepositoryFindResult<UserAndInfo>> FindByOdsCode(string odsCode)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(odsCode, nameof(odsCode), ValidateAndLog.ValidationOptions.ThrowError)
                    .IsValid();

                return await _repository.Find(d => d.Info.OdsCode == odsCode, RecordName);

            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<RepositoryFindResult<UserAndInfo>> FindByNhsNumber(string nhsNumber)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(nhsNumber, nameof(nhsNumber), ValidateAndLog.ValidationOptions.ThrowError)
                    .IsValid();

                return await _repository.Find(d => d.Info.NhsNumber == nhsNumber, RecordName);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
