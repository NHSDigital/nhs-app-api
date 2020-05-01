using System.Threading.Tasks;

namespace NHSOnline.Backend.PfsApi.Session
{
    public interface ISessionCreator
    {
        Task<CreateSessionResult> CreateSession(ICreateSessionRequest request);
    }
}