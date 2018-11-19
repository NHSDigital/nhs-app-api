using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.GpSystems.Session
{
    public interface ISessionExtendService
    {
        Task<SessionExtendResult> Extend(UserSession userSession);
    }
}