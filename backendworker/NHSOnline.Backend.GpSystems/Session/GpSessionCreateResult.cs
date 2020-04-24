using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Session
{
    public abstract class GpSessionCreateResult
    {
        private GpSessionCreateResult()
        {
        }

        public abstract T Accept<T>(IGpSessionCreateResultVisitor<T> visitor);
        
        public sealed class Success : GpSessionCreateResult
        {
            public GpUserSession UserSession { get; }

            public Success(GpUserSession userSession)
            {
                UserSession = userSession;
            }

            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public sealed class Forbidden : GpSessionCreateResult
        {
            public Forbidden(string message) => Message = message;
            public string Message { get; }

            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class BadGateway : GpSessionCreateResult
        {
            public BadGateway(string message) => Message = message;
            public string Message { get; }

            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor) => visitor.Visit(this);
        }
        
        public sealed class BadRequest : GpSessionCreateResult
        {
            public BadRequest(string message) => Message = message;
            public string Message { get; }

            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class InternalServerError : GpSessionCreateResult
        {
            public InternalServerError(string message) => Message = message;
            public string Message { get; }

            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class InvalidConnectionToken : GpSessionCreateResult
        {
            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}