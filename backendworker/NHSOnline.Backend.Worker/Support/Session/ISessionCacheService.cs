using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.Support.Session
{
    public interface ISessionCacheService
    {
        Task<string> CreateUserSession(UserSession userSession);

        Task<Option<UserSession>> GetUserSession(string sessionId);

        Task<bool> DeleteUserSession(string sessionId);

        Task UpdateUserSession(UserSession userSession);
    }
}