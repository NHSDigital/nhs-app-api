using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.LinkedAccounts
{
    [FakeGpAreaBehaviour(Behaviour.Unauthorised)]
    public class UnauthorisedLinkedAccountsAreaBehaviour: ILinkedAccountsAreaBehaviour
    {
        public string GetOdsCodeForLinkedAccount(GpUserSession gpUserSession, Guid id) =>
            throw new UnauthorisedGpSystemHttpRequestException();

        public Task<SwitchAccountResult> SwitchAccount(GpLinkedAccountModel gpLinkedAccountModel) =>
            throw new UnauthorisedGpSystemHttpRequestException();

        public Task<LinkedAccountsResult> GetLinkedAccounts(GpUserSession gpUserSession) =>
            throw new UnauthorisedGpSystemHttpRequestException();

        public Task<LinkedAccountAccessSummaryResult> GetLinkedAccount(GpUserSession gpUserSession, Guid id) =>
            throw new UnauthorisedGpSystemHttpRequestException();

        public LinkedAccountAuditInfo GetProxyAuditData(GpLinkedAccountModel gpLinkedAccountModel) =>
            throw new UnauthorisedGpSystemHttpRequestException();

        public string GetNhsNumberForProxyUser(GpUserSession gpUserSession, Guid id) =>
            throw new UnauthorisedGpSystemHttpRequestException();
    }
}