namespace NHSOnline.Backend.Worker.GpSystems.Session
{
    public abstract class SessionLogoffResult
    {
        private SessionLogoffResult()
        {
        }

        public abstract T Accept<T>(ISessionLogoffResultVisitor<T> visitor);

        public class SuccessfullyDeleted : SessionLogoffResult
        {
            public UserSession UserSession { get; }

            public SuccessfullyDeleted(
                UserSession userSession)
            {
                UserSession = userSession;
            }

            public override T Accept<T>(ISessionLogoffResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotAuthenticated : SessionLogoffResult
        {
            public override T Accept<T>(ISessionLogoffResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SupplierSystemUnavailable : SessionLogoffResult
        {
            public override T Accept<T>(ISessionLogoffResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}