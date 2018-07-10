namespace NHSOnline.Backend.Worker.GpSystems.Session
{
    public abstract class SessionCreateResult
    {
        private SessionCreateResult()
        {
        }

        public abstract T Accept<T>(ISessionCreateResultVisitor<T> visitor);

        public class SuccessfullyCreated : SessionCreateResult
        {
            public UserSession UserSession { get; }
            public string Name { get; }

            public SuccessfullyCreated(
                string name, 
                UserSession userSession)
            {
                Name = name;
                UserSession = userSession;
            }

            public override T Accept<T>(ISessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InvalidIm1ConnectionToken : SessionCreateResult
        {
            public override T Accept<T>(ISessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SupplierSystemUnavailable : SessionCreateResult
        {
            public override T Accept<T>(ISessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}