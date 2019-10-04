using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Repository;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo;

namespace NHSOnline.Backend.UserInfoApi.Repository
{
    [SuppressMessage("Microsoft.Globalization", "CA1309",
        Justification = "Method �CompareOrdinal� is not supported on Mongo Driver")]
    internal class MongoUserInfoRepository : MongoRepository<UserAndInfo>, IInfoRepository
    {
        private readonly ILogger<MongoUserInfoRepository> _logger;

        public MongoUserInfoRepository(
            ILogger<MongoUserInfoRepository> logger,
            IMongoClient mongoClient,
            IMongoConfiguration mongoConfiguration
        )
            : base(mongoClient, mongoConfiguration)
        {
            _logger = logger;
        }

        public async Task<PostInfoResult> Create(UserAndInfo userAndInfo)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(userAndInfo, nameof(userAndInfo), ValidateAndLog.ValidationOptions.ThrowError)
                    .IsValid();

                await CreateOrUpdateOneAsync(d => d.NhsLoginId == userAndInfo.NhsLoginId, userAndInfo);
                return new PostInfoResult.Created();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<UserAndInfo> FindByNhsLoginId(string nhsLoginId)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(nhsLoginId, nameof(nhsLoginId), ValidateAndLog.ValidationOptions.ThrowError)
                    .IsValid();

                using (_logger.WithTimer("Find user info in Mongo"))
                {
                    return await FindOneAsync(d => d.NhsLoginId == nhsLoginId);
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<IEnumerable<UserAndInfo>> FindByOdsCode(string odsCode)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(odsCode, nameof(odsCode), ValidateAndLog.ValidationOptions.ThrowError)
                    .IsValid();

                using (_logger.WithTimer("Find user info in Mongo"))
                {
                    return await FindMultipleAsync(d => d.Info.OdsCode == odsCode);
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }
        
        public async Task<IEnumerable<UserAndInfo>> FindByNhsNumber(string nhsNumber)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(nhsNumber, nameof(nhsNumber), ValidateAndLog.ValidationOptions.ThrowError)
                    .IsValid();

                using (_logger.WithTimer("Find user info in Mongo"))
                {
                    return await FindMultipleAsync(d => d.Info.NhsNumber == nhsNumber);
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}