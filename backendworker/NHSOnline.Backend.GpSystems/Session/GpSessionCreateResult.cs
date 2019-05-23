using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Session
{
    public abstract class GpSessionCreateResult
    {
        private GpSessionCreateResult()
        {
        }

        public abstract T Accept<T>(IGpSessionCreateResultVisitor<T> visitor);

        public class Success : GpSessionCreateResult
        {
            public GpUserSession UserSession { get; }
            public string Name { get; }

            public Success(
                string name, 
                GpUserSession userSession)
            {
                Name = name;
                UserSession = userSession;
            }

            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Forbidden : GpSessionCreateResult
        {
            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : GpSessionCreateResult
        {
            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadRequest : GpSessionCreateResult
        {
            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : GpSessionCreateResult
        {
            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}