using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.GpSystems.Session
{
    public interface ISessionService
    {
        Task<SessionCreateResult> Create(string im1ConnectionToken, string odsCode);
    }
}
