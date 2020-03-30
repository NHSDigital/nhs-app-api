using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Session
{
    public class VisionSessionExtendService : ISessionExtendService
    {
        public Task<SessionExtendResult> Extend(GpLinkedAccountModel gpLinkedAccountModel)
        {
            return Task.FromResult((SessionExtendResult)new SessionExtendResult.Success());
        }
    }
}