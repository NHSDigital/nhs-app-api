namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public abstract class CloseSessionResult
    {
        public class Success : CloseSessionResult
        {
        }

        public class Failure : CloseSessionResult
        {
        }
    }
}