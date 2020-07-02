using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users
{
    public interface IFakeUserRepository
    {
        Task<FakeUser> Find(string nhsNumber);
    }

    public class FakeUserRepository : IFakeUserRepository
    {
        private const string DynamicUserGroup = "DYNAMIC";

        private readonly IServiceProvider _serviceProvider;
        private readonly IRepository<FakeUser> _fakeGpUsersRepo;
        private readonly IDictionary<string, FakeUser> _defaultUsers;
        private readonly ILogger<FakeUserRepository> _logger;

        public FakeUserRepository(
            IServiceProvider serviceProvider,
            IRepository<FakeUser> fakeGpUsersRepo,
            IOptions<FakeGpSupplierConfiguration> fakeGpSupplierConfiguration,
            ILogger<FakeUserRepository> logger
        )
        {
            _serviceProvider = serviceProvider;
            _fakeGpUsersRepo = fakeGpUsersRepo;
            _logger = logger;

            _defaultUsers = new Dictionary<string, FakeUser>();

            InitDefaultUsers(fakeGpSupplierConfiguration.Value);
        }

        public async Task<FakeUser> Find(string nhsNumber)
        {
            if (string.IsNullOrEmpty(nhsNumber))
            {
                throw new UnknownFakeUserException(
                    $"Unable to find Fake User because NHS Number was null/blank"
                );
            }

            var trimmedNhsNumber = nhsNumber.RemoveWhiteSpace();
            var fakeUser = FindFakeUser(trimmedNhsNumber);

            if (fakeUser.GroupName != DynamicUserGroup)
            {
                return fakeUser;
            }

            var repoUserOverride = await FindFakeUserOverride(trimmedNhsNumber);

            if (repoUserOverride is null)
            {
                await _fakeGpUsersRepo.Create(fakeUser, nameof(FakeUser));
            }

            return repoUserOverride ?? fakeUser;
        }

        private void InitDefaultUsers(FakeGpSupplierConfiguration config)
        {
            foreach (var (userGroupName, groupUsers) in config.DefaultUsers)
            {
                var uppercaseUserGroupName = userGroupName.ToUpperInvariant();

                foreach (var (nhsNumber, user) in groupUsers)
                {
                    user.GroupName = uppercaseUserGroupName;
                    user.NhsNumber = nhsNumber;
                    user.ServiceProvider = _serviceProvider;

                    _defaultUsers[nhsNumber] = user;
                }
            }
        }

        private async Task<FakeUser> FindFakeUserOverride(string nhsNumber)
        {
            _logger.LogInformation(
                $"User in dynamic group detected. Attempting to find override for Fake User with NHS Number: '{nhsNumber}'."
            );

            var repoUsers = await _fakeGpUsersRepo.Find(
                u => u.NhsNumber == nhsNumber,
                nameof(FakeUser)
            );

            var repoUsersVisitor = new FakeGpUserRepoResultVisitor(_serviceProvider);

            var repoUser = repoUsers.Accept(repoUsersVisitor);

            _logger.LogInformation(repoUser is null
                ? $"No override found for Fake User with NHS Number: '{nhsNumber}'."
                : $"Found override for Fake User with NHS Number: {nhsNumber} | User Group: {repoUser.GroupName}"
            );

            return repoUser;
        }

        private FakeUser FindFakeUser(string nhsNumber)
        {
            _logger.LogInformation($"Attempting to find Fake User with NHS Number: '{nhsNumber}'.");

            if (!_defaultUsers.ContainsKey(nhsNumber))
            {
                throw new UnknownFakeUserException($"Unable to find Fake User with NHS Number: '{nhsNumber}'.");
            }

            var fakeUser = _defaultUsers[nhsNumber];

            _logger.LogInformation(
                $"Found Fake User with NHS Number: {nhsNumber} | User Group: {fakeUser.GroupName}"
            );

            return fakeUser;
        }
    }
}