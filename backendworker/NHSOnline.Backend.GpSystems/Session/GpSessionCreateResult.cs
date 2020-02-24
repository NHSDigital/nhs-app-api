using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Session
{
    public abstract class GpSessionCreateResult
    {
        private GpSessionCreateResult()
        {
        }

        public int StatusCode { get; set; }
        
        public abstract T Accept<T>(IGpSessionCreateResultVisitor<T> visitor);
        
        public class Success : GpSessionCreateResult
        {
            public GpUserSession UserSession { get; }

            public Success(GpUserSession userSession)
            {
                UserSession = userSession;
                StatusCode = StatusCodes.Status200OK;
            }

            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Forbidden : GpSessionCreateResult
        {
            public Forbidden() 
            {
                StatusCode = StatusCodes.Status403Forbidden;
            }

            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : GpSessionCreateResult
        {
            public BadGateway() 
            {
                StatusCode = StatusCodes.Status502BadGateway;
            }

            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadRequest : GpSessionCreateResult
        {
            public BadRequest() 
            {
                StatusCode = StatusCodes.Status400BadRequest;
            }

            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : GpSessionCreateResult
        {
            public InternalServerError() 
            {
                StatusCode = StatusCodes.Status500InternalServerError;
            }

            public override T Accept<T>(IGpSessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}