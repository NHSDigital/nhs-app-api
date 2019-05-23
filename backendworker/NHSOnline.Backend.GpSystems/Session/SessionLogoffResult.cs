using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Session
{
    public abstract class SessionLogoffResult
    {
        private SessionLogoffResult()
        {
        }

        public abstract T Accept<T>(ISessionLogoffResultVisitor<T> visitor);

        public class Success : SessionLogoffResult
        {
            public GpUserSession GpUserSession { get; }

            public Success(
                GpUserSession gpUserSession)
            {
                GpUserSession = gpUserSession;
            }

            public override T Accept<T>(ISessionLogoffResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Forbidden : SessionLogoffResult
        {
            public override T Accept<T>(ISessionLogoffResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : SessionLogoffResult
        {
            public override T Accept<T>(ISessionLogoffResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}