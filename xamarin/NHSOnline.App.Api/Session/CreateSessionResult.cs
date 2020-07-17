using System.Net;

namespace NHSOnline.App.Api.Session
{
    public abstract class CreateSessionResult
    {
        private CreateSessionResult() {}

        public abstract T Accept<T>(ICreateSessionResultVisitor<T> visitor);

        public sealed class Created : CreateSessionResult
        {
            public Created(UserSession userSession, CookieContainer cookies)
            {
                UserSession = userSession;
                Cookies = cookies;
            }

            public UserSession UserSession { get; }
            public CookieContainer Cookies { get; }

            public override T Accept<T>(ICreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class BadRequest : CreateSessionResult
        {
            public BadRequest(string serviceDeskReference) => ServiceDeskReference = serviceDeskReference;

            public string ServiceDeskReference { get; }

            public override T Accept<T>(ICreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class Forbidden : CreateSessionResult
        {
            public Forbidden(string serviceDeskReference) => ServiceDeskReference = serviceDeskReference;

            public string ServiceDeskReference { get; }

            public override T Accept<T>(ICreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class OdsCodeNotSupportedOrNoNhsNumber : CreateSessionResult
        {
            public OdsCodeNotSupportedOrNoNhsNumber(string serviceDeskReference) => ServiceDeskReference = serviceDeskReference;

            public string ServiceDeskReference { get; }

            public override T Accept<T>(ICreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class Failed : CreateSessionResult
        {
            public override T Accept<T>(ICreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}