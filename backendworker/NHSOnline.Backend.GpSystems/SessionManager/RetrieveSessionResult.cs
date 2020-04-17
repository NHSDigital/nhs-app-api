using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public abstract class RetrieveSessionResult
    {
        public sealed class Success : RetrieveSessionResult
        {
            public Success(UserSession userSession) => UserSession = userSession;

            public UserSession UserSession { get; }
        }

        public sealed class Failure : RetrieveSessionResult
        {
        }
    }
}