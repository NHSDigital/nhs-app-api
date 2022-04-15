using System;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Repository.SqlApi;

namespace NHSOnline.Backend.UserInfo.Repository
{
    public class UserAndInfoSqlApiRepositoryFactory : IUserAndInfoSqlApiRepositoryFactory
    {
        public enum UserAndInfoRepositoryKey
        {
            NhsNumber,
            OdsCode
        }

        private readonly IServiceProvider _serviceProvider;

        public UserAndInfoSqlApiRepositoryFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ISqlApiRepository<UserAndInfo> GetUserAndInfoSqlApiRepository(UserAndInfoRepositoryKey partitionKey)
        {
            return partitionKey switch
            {
                UserAndInfoRepositoryKey.NhsNumber => (ISqlApiRepository<UserAndInfo>) _serviceProvider.GetRequiredService(
                    typeof(SqlApiRepository<UserAndInfoRepositoryByNhsNoConfiguration, UserAndInfo>)),
                UserAndInfoRepositoryKey.OdsCode => (ISqlApiRepository<UserAndInfo>) _serviceProvider.GetRequiredService(
                    typeof(SqlApiRepository<UserAndInfoRepositoryByOdsCodeConfiguration, UserAndInfo>)),
                _ => throw new ArgumentOutOfRangeException(nameof(partitionKey), partitionKey,
                    @"No repo was found for this partition key")
            };
        }
    }
}