namespace NHSOnline.Backend.GpSystems.Session
{
    public abstract class GpSessionRecreateResult
    {
        public class Success : GpSessionRecreateResult
        {
            public string Suid { get; set; }
            public bool HasSelfAccess { get; set; }

            public Success(string suid, bool hasSelfAccess)
            {
                Suid = suid;
                HasSelfAccess = hasSelfAccess;
            }
        }

        public class Failure : GpSessionRecreateResult
        {
        }
    }
}