using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.LinkedAccounts
{
    public interface ILinkedAccountsService
    {
        string GetOdsCodeForLinkedAccount(GpUserSession gpUserSession, Guid id);

        bool IsValidLinkedAccountId(GpUserSession gpUserSession, Guid id);

        Task<LinkedAccountsResult> GetLinkedAccounts(GpUserSession gpUserSession);

        Task<LinkedAccountAccessSummaryResult> GetLinkedAccount(GpUserSession gpUserSession, Guid id);
    }
}
