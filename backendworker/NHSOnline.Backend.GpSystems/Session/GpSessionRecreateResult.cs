namespace NHSOnline.Backend.GpSystems.Session
{
    public abstract class GpSessionRecreateResult
    {
        public class Success : GpSessionRecreateResult
        {
            public string Suid { get; set; }

            public Success(string suid)
            {
                Suid = suid;
            }
        }

        public class Failure : GpSessionRecreateResult
        {
        }
    }
}