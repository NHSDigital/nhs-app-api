using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Session
{
    public abstract class GpSessionCreateResult
    {
        private GpSessionCreateResult()
        {
        }

        public abstract T Accept<T>(IGpSessionCreateResultVisitor<T> visitor);

        public class SuccessfullyCreated : GpSessionCreateResult
        {
            public GpUserSession UserSession { get; }
            public string Name { get; }

            public SuccessfullyCreated(
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

        public class InvalidIm1ConnectionToken : GpSessionCreateResult
        {
            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SupplierSystemUnavailable : GpSessionCreateResult
        {
            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class SupplierSystemBadResponse : GpSessionCreateResult
        {
            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InvalidRequest : GpSessionCreateResult
        {
            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class ErrorProcessingSecurityHeader : GpSessionCreateResult
        {
            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InvalidUserCredentials : GpSessionCreateResult
        {
            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class UnknownError : GpSessionCreateResult
        {
            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}