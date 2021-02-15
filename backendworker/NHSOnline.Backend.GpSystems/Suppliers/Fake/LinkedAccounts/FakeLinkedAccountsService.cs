using System;
using System.Collections.Generic;
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

        public string GetOdsCodeForLinkedAccount(GpUserSession gpUserSession, string patientGpIdentifier)
        {
            _logger.LogEnter();

            var fakeUser = FindUser(gpUserSession).Result;
            return fakeUser.LinkedAccountsAreaBehaviour.GetOdsCodeForLinkedAccount(gpUserSession, patientGpIdentifier);
        }

        public async Task<SwitchAccountResult> SwitchAccount(GpUserSession gpUserSession, string patientGpIdentifier)
        {
            _logger.LogEnter();

            var fakeUser = await FindUser(gpUserSession);
            return await fakeUser.LinkedAccountsAreaBehaviour.SwitchAccount(gpUserSession, patientGpIdentifier);
        }

        public async Task<LinkedAccountsResult> GetLinkedAccounts(GpUserSession gpUserSession, Dictionary<Guid, string> gpIdentifierMapping)
        {
            _logger.LogEnter();

            var fakeUser = await FindUser(gpUserSession);
            return await fakeUser.LinkedAccountsAreaBehaviour.GetLinkedAccounts(gpUserSession, gpIdentifierMapping);
        }

        public async Task<LinkedAccountAccessSummaryResult> GetLinkedAccount(GpUserSession gpUserSession, string patientGpIdentifier)
        {
            _logger.LogEnter();

            var fakeUser = await FindUser(gpUserSession);
            return await fakeUser.LinkedAccountsAreaBehaviour.GetLinkedAccount(gpUserSession, patientGpIdentifier);
        }

        public LinkedAccountAuditInfo GetProxyAuditData(GpLinkedAccountModel gpLinkedAccountModel)
        {
            _logger.LogEnter();

            var fakeUser = FindUser(gpLinkedAccountModel.GpUserSession).Result;
            return fakeUser.LinkedAccountsAreaBehaviour.GetProxyAuditData(gpLinkedAccountModel);
        }

        public string GetNhsNumberForProxyUser(GpUserSession gpUserSession, string patientGpIdentifier)
        {
            _logger.LogEnter();

            var fakeUser = FindUser(gpUserSession).Result;
            return fakeUser.LinkedAccountsAreaBehaviour.GetNhsNumberForProxyUser(gpUserSession, patientGpIdentifier);
        }
    }
}