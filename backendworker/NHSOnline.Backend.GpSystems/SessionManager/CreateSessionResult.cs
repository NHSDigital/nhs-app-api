using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public abstract class CreateSessionResult
    {
        public int StatusCode { get; set; }

        public class Success : CreateSessionResult
        {
            public P9UserSession UserSession { get; }
            public Success(P9UserSession userSession)
            {
                UserSession = userSession;
            }
        }

        public class Failure : CreateSessionResult
        {
            public Failure(int statusCode)
            {
                StatusCode = statusCode;
            }
        }
    }
}