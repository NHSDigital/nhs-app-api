using NHSOnline.Backend.Worker.Areas.Session.Models;

namespace NHSOnline.Backend.Worker.Router.Session
{
    public abstract class SessionCreateResult
    {
        private SessionCreateResult()
        {
        }

        public abstract T Accept<T>(ISessionCreateResultVisitor<T> visitor);

        public class SuccessfullyCreated : SessionCreateResult
        {
            public UserSessionResponse UserInfo { get; }
            public string SessionId { get; set; }

            public SuccessfullyCreated(string sessionId, UserSessionResponse userInfo)
            {
                UserInfo = userInfo;
                SessionId = sessionId;
            }

            public override T Accept<T>(ISessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InvalidIm1ConnectionToken : SessionCreateResult
        {
            public override T Accept<T>(ISessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SupplierSystemUnavailable : SessionCreateResult
        {
            public override T Accept<T>(ISessionCreateResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}