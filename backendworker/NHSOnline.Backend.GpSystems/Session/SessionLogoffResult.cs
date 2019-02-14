using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Session
{
    public abstract class SessionLogoffResult
    {
        private SessionLogoffResult()
        {
        }

        public abstract T Accept<T>(ISessionLogoffResultVisitor<T> visitor);

        public class SuccessfullyDeleted : SessionLogoffResult
        {
            public GpUserSession GpUserSession { get; }

            public SuccessfullyDeleted(
                GpUserSession gpUserSession)
            {
                GpUserSession = gpUserSession;
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