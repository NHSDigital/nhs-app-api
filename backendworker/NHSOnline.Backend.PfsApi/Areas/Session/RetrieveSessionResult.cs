using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    public abstract class RetrieveSessionResult
    {
        public class Success : RetrieveSessionResult
        {
            public UserSession UserSession { get; }
            public Success(UserSession userSession)
            {
                UserSession = userSession;
            }
        }
        
        public class Failure : RetrieveSessionResult
        {
        }  
    }
}