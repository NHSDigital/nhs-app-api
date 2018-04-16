using System;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.Session
{
    public interface ISessionCacheService
    {
        Task<string> CreateUserSession(UserSession userSession);
    }

    public class SessionCacheService : ISessionCacheService
    {
        // TODO - NHSO-457
        public Task<string> CreateUserSession(UserSession userSession)
        {
            throw new NotImplementedException();
        }
    }
}
