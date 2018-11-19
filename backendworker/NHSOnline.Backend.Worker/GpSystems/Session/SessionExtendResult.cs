namespace NHSOnline.Backend.Worker.GpSystems.Session
{
    public abstract class SessionExtendResult
    {
        private SessionExtendResult(){}

        public abstract T Accept<T>(ISessionExtendResultVisitor<T> visitor);

        public class SuccessfullyExtended : SessionExtendResult
        {
            public override T Accept<T>(ISessionExtendResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SupplierSystemUnavailable : SessionExtendResult
        {
            public override T Accept<T>(ISessionExtendResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}