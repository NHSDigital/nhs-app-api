using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.LinkedAccounts
{
    [FakeGpArea("LinkedAccounts")]
    public interface ILinkedAccountsAreaBehaviour
    {
        string GetOdsCodeForLinkedAccount(GpUserSession gpUserSession, Guid id);

        Task<SwitchAccountResult> SwitchAccount(GpLinkedAccountModel gpLinkedAccountModel);

        Task<LinkedAccountsResult> GetLinkedAccounts(GpUserSession gpUserSession);

        Task<LinkedAccountAccessSummaryResult> GetLinkedAccount(GpUserSession gpUserSession, Guid id);

        LinkedAccountAuditInfo GetProxyAuditData(GpLinkedAccountModel gpLinkedAccountModel);

        string GetNhsNumberForProxyUser(GpUserSession gpUserSession, Guid id);
    }
}