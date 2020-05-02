namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public abstract class CloseSessionResult
    {
        public abstract T Accept<T>(ICloseSessionResultVisitor<T> visitor);

        public sealed class Success : CloseSessionResult
        {
            public override T Accept<T>(ICloseSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class Failure : CloseSessionResult
        {
            public override T Accept<T>(ICloseSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}