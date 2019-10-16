using System.Threading.Tasks;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.LinkedAccounts
{
    public interface ILinkedAccountsService
    {
        Task<LinkedAccountsResult> GetLinkedAccounts(GpUserSession gpUserSession);
    }
}
