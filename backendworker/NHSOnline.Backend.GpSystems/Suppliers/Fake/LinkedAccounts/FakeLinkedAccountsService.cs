using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Users;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.LinkedAccounts
{
    public class FakeLinkedAccountsService : FakeServiceBase, ILinkedAccountsService
    {
        private readonly ILogger<FakeLinkedAccountsService> _logger;

        public FakeLinkedAccountsService(ILogger<FakeLinkedAccountsService> logger,
            IFakeUserRepository fakeUserRepository)
            : base(logger, fakeUserRepository)
        {
            _logger = logger;
        }

        public string GetOdsCodeForLinkedAccount(GpUserSession gpUserSession, Guid id)
        {
            _logger.LogEnter();

            var fakeUser = FindUser(gpUserSession.NhsNumber).Result;
            return fakeUser.LinkedAccountsAreaBehaviour.GetOdsCodeForLinkedAccount(gpUserSession, id);
        }

        public async Task<SwitchAccountResult> SwitchAccount(GpLinkedAccountModel gpLinkedAccountModel)
        {
            _logger.LogEnter();

            var fakeUser = await FindUser(gpLinkedAccountModel);
            return await fakeUser.LinkedAccountsAreaBehaviour.SwitchAccount(gpLinkedAccountModel);
        }

        public async Task<LinkedAccountsResult> GetLinkedAccounts(GpUserSession gpUserSession)
        {
            _logger.LogEnter();

            var fakeUser = await FindUser(gpUserSession.NhsNumber);
            return await fakeUser.LinkedAccountsAreaBehaviour.GetLinkedAccounts(gpUserSession);
        }

        public async Task<LinkedAccountAccessSummaryResult> GetLinkedAccount(GpUserSession gpUserSession, Guid id)
        {
            _logger.LogEnter();

            var fakeUser = await FindUser(gpUserSession.NhsNumber);
            return await fakeUser.LinkedAccountsAreaBehaviour.GetLinkedAccount(gpUserSession, id);
        }

        public LinkedAccountAuditInfo GetProxyAuditData(GpLinkedAccountModel gpLinkedAccountModel)
        {
            _logger.LogEnter();

            var fakeUser = FindUser(gpLinkedAccountModel).Result;
            return fakeUser.LinkedAccountsAreaBehaviour.GetProxyAuditData(gpLinkedAccountModel);
        }

        public string GetNhsNumberForProxyUser(GpUserSession gpUserSession, Guid id)
        {
            _logger.LogEnter();

            var fakeUser = FindUser(gpUserSession.NhsNumber).Result;
            return fakeUser.LinkedAccountsAreaBehaviour.GetNhsNumberForProxyUser(gpUserSession, id);
        }
    }
}