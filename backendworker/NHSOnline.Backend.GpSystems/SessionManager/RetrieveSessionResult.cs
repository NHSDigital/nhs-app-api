using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public abstract class RetrieveSessionResult
    {
        public class Success : RetrieveSessionResult
        {
            public P9UserSession UserSession { get; }
            public Success(P9UserSession userSession)
            {
                UserSession = userSession;
            }
        }

        public class Failure : RetrieveSessionResult
        {
        }
    }
}