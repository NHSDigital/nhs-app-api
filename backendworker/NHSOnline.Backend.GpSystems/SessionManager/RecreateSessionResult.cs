using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public abstract class RecreateSessionResult
    {
        public class Success : RecreateSessionResult
        {
            public  P9UserSession UserSession { get; }

            public Success(P9UserSession userSession)
            {
                UserSession = userSession;
            }
        }

        public class Failure : RecreateSessionResult
        {
        }
    }
}