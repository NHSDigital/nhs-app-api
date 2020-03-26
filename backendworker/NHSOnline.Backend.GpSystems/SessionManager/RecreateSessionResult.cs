using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public abstract class RecreateSessionResult
    {
        public class Success : RecreateSessionResult
        {
            public  UserSession UserSession { get; }

            public Success(UserSession userSession)
            {
                UserSession = userSession;
            }
        }

        public class Failure : RecreateSessionResult
        {
        }
    }
}