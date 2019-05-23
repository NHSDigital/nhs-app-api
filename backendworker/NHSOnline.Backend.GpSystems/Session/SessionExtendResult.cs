namespace NHSOnline.Backend.GpSystems.Session
{
    public abstract class SessionExtendResult
    {
        private SessionExtendResult(){}

        public abstract T Accept<T>(ISessionExtendResultVisitor<T> visitor);

        public class Success : SessionExtendResult
        {
            public override T Accept<T>(ISessionExtendResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : SessionExtendResult
        {
            public override T Accept<T>(ISessionExtendResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}