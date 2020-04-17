using System.Threading.Tasks;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public interface ISessionCacheService
    {
        Task<string> CreateUserSession(UserSession userSession);
        Task<Option<UserSession>> GetUserSession(string sessionId);
        Task<bool> DeleteUserSession(string sessionId);
        Task UpdateUserSession(UserSession userSession);
    }
}