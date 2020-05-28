using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
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

            var fakeUser = FindUser(gpUserSession.NhsNumber);
            return fakeUser.LinkedAccountsBehaviour.GetOdsCodeForLinkedAccount(gpUserSession, id);
        }

        public async Task<SwitchAccountResult> SwitchAccount(GpLinkedAccountModel gpLinkedAccountModel)
        {
            _logger.LogEnter();

            var fakeUser = FindUser(gpLinkedAccountModel);
            return await fakeUser.LinkedAccountsBehaviour.SwitchAccount(gpLinkedAccountModel);
        }

        public async Task<LinkedAccountsResult> GetLinkedAccounts(GpUserSession gpUserSession)
        {
            _logger.LogEnter();

            var fakeUser = FindUser(gpUserSession.NhsNumber);
            return await fakeUser.LinkedAccountsBehaviour.GetLinkedAccounts(gpUserSession);
        }

        public async Task<LinkedAccountAccessSummaryResult> GetLinkedAccount(GpUserSession gpUserSession, Guid id)
        {
            _logger.LogEnter();

            var fakeUser = FindUser(gpUserSession.NhsNumber);
            return await fakeUser.LinkedAccountsBehaviour.GetLinkedAccount(gpUserSession, id);
        }

        public LinkedAccountAuditInfo GetProxyAuditData(GpLinkedAccountModel gpLinkedAccountModel)
        {
            _logger.LogEnter();

            var fakeUser = FindUser(gpLinkedAccountModel);
            return fakeUser.LinkedAccountsBehaviour.GetProxyAuditData(gpLinkedAccountModel);
        }

        public string GetNhsNumberForProxyUser(GpUserSession gpUserSession, Guid id)
        {
            _logger.LogEnter();

            var fakeUser = FindUser(gpUserSession.NhsNumber);
            return fakeUser.LinkedAccountsBehaviour.GetNhsNumberForProxyUser(gpUserSession, id);
        }
    }
}