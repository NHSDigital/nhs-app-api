using System.Threading.Tasks;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public interface ISessionCacheService
    {
        Task<string> CreateUserSession(P9UserSession userSession);
        Task<Option<P9UserSession>> GetUserSession(string sessionId);
        Task<bool> DeleteUserSession(string sessionId);
        Task UpdateUserSession(P9UserSession userSession);
    }
}