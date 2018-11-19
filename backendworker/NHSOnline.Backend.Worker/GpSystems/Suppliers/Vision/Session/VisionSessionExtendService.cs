using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.Session;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session
{
    public class VisionSessionExtendService : ISessionExtendService
    {
        public Task<SessionExtendResult> Extend(UserSession userSession)
        {
            return Task.FromResult((SessionExtendResult)new SessionExtendResult.SuccessfullyExtended());
        }
    }
}