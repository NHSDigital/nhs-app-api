using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.LinkedAccounts
{
    public interface ILinkedAccountsService
    {
        string GetOdsCodeForLinkedAccount(GpUserSession gpUserSession, string patientGpIdentifier);

        Task<SwitchAccountResult> SwitchAccount(GpUserSession gpUserSession, string patientGpIdentifier);

        Task<LinkedAccountsResult> GetLinkedAccounts(GpUserSession gpUserSession, Dictionary<Guid, string> gpIdentifierMapping);

        Task<LinkedAccountAccessSummaryResult> GetLinkedAccount(GpUserSession gpUserSession, string patientGpIdentifier);

        LinkedAccountAuditInfo GetProxyAuditData(GpLinkedAccountModel gpLinkedAccountModel);

        string GetNhsNumberForProxyUser(GpUserSession gpUserSession, string patientGpIdentifier);
    }
}
