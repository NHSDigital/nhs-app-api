using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.UserInfo.Repository
{
    [SuppressMessage("Microsoft.Globalization", "CA1309",
        Justification = "Method �CompareOrdinal� is not supported in repository")]
    internal class UserInfoRepository : IInfoRepository
    {
        private readonly ILogger<UserInfoRepository> _logger;
        private readonly IRepository<UserAndInfo> _primaryRepository;
        private readonly IUserAndInfoSqlApiRepositoryFactory _sqlApiRepositoryFactory;
        private const string RecordName = nameof(UserAndInfo);

        public UserInfoRepository(
            ILogger<UserInfoRepository> logger,
            IRepository<UserAndInfo> primaryRepository,
            IUserAndInfoSqlApiRepositoryFactory sqlApiRepositoryFactory
        )
        {
            _logger = logger;
            _primaryRepository = primaryRepository;
            _sqlApiRepositoryFactory = sqlApiRepositoryFactory;
        }

        public async Task<RepositoryCreateResult<UserAndInfo>> CreateOrUpdatePrimary(UserAndInfo userAndInfo)
        {
            return await CreateOrUpdateRecord(userAndInfo,
                () => _primaryRepository
                .CreateOrUpdate(d => d.NhsLoginId == userAndInfo.NhsLoginId,
                    userAndInfo,
                    RecordName));
        }

        public async Task<RepositoryCreateResult<UserAndInfo>> CreateOrUpdateNhsNumberRecord(UserAndInfo userAndInfo)
        {
            return await CreateOrUpdateRecord(userAndInfo,
                () => _sqlApiRepositoryFactory
                    .GetUserAndInfoSqlApiRepository(UserAndInfoSqlApiRepositoryFactory.UserAndInfoRepositoryKey.NhsNumber)
                    .CreateOrUpdate(userAndInfo, RecordName, userAndInfo.Info.NhsNumber));
        }

        public async Task<RepositoryCreateResult<UserAndInfo>> CreateOrUpdateOdsCodeRecord(UserAndInfo userAndInfo)
        {
            return await CreateOrUpdateRecord(userAndInfo,
                () => _sqlApiRepositoryFactory
                    .GetUserAndInfoSqlApiRepository(UserAndInfoSqlApiRepositoryFactory.UserAndInfoRepositoryKey.OdsCode)
                    .CreateOrUpdate(userAndInfo, RecordName, userAndInfo.Info.OdsCode));
        }

        public async Task<RepositoryDeleteResult<UserAndInfo>> DeleteNhsNumberRecord(string nhsNumber, string nhsLoginId)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNullOrEmpty(nhsNumber, nameof(nhsNumber), ValidateAndLog.ValidationOptions.ThrowError)
                    .IsNotNullOrEmpty(nhsLoginId, nameof(nhsLoginId), ValidateAndLog.ValidationOptions.ThrowError)
                    .IsValid();

                return await _sqlApiRepositoryFactory
                    .GetUserAndInfoSqlApiRepository(UserAndInfoSqlApiRepositoryFactory.UserAndInfoRepositoryKey.NhsNumber)
                    .Delete(nhsLoginId, nhsNumber, RecordName);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<RepositoryDeleteResult<UserAndInfo>> DeleteOdsCodeRecord(string odsCode, string nhsLoginId)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNullOrEmpty(odsCode, nameof(odsCode), ValidateAndLog.ValidationOptions.ThrowError)
                    .IsNotNullOrEmpty(nhsLoginId, nameof(nhsLoginId), ValidateAndLog.ValidationOptions.ThrowError)
                    .IsValid();

                return await _sqlApiRepositoryFactory
                    .GetUserAndInfoSqlApiRepository(UserAndInfoSqlApiRepositoryFactory.UserAndInfoRepositoryKey.OdsCode)
                    .Delete(nhsLoginId, odsCode, RecordName);
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

                return await _primaryRepository
                    .Find(d => d.NhsLoginId == nhsLoginId, RecordName);

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

                return await _sqlApiRepositoryFactory
                    .GetUserAndInfoSqlApiRepository(UserAndInfoSqlApiRepositoryFactory.UserAndInfoRepositoryKey.OdsCode)
                    .Find(x => x.Info.OdsCode == odsCode, odsCode, RecordName);
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

                return await _sqlApiRepositoryFactory
                    .GetUserAndInfoSqlApiRepository(UserAndInfoSqlApiRepositoryFactory.UserAndInfoRepositoryKey.NhsNumber)
                    .Find(x => x.Info.NhsNumber == nhsNumber, nhsNumber, RecordName);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<RepositoryCreateResult<UserAndInfo>> CreateOrUpdateRecord(
            UserAndInfo userAndInfo,
            Func<Task<RepositoryCreateResult<UserAndInfo>>> createOrUpdateRecord)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(userAndInfo, nameof(userAndInfo), ValidateAndLog.ValidationOptions.ThrowError)
                    .IsValid();

                return await createOrUpdateRecord();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
