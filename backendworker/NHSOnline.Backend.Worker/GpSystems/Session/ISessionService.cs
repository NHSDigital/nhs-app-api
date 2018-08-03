using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.GpSystems.Session
{
    public interface ISessionService
    {
        Task<SessionCreateResult> Create(string connectionToken, string odsCode);
    }
}
