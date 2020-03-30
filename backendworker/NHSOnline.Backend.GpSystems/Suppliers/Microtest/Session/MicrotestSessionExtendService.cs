using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Session
{
    public class MicrotestSessionExtendService : ISessionExtendService
    {
        public Task<SessionExtendResult> Extend(GpLinkedAccountModel gpLinkedAccountModel)
        {
            return Task.FromResult((SessionExtendResult)new SessionExtendResult.Success());
        }
    }
}
