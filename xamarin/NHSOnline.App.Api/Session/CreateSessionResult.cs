using NHSOnline.App.Api.Client.Cookies;

namespace NHSOnline.App.Api.Session
{
    public abstract class CreateSessionResult
    {
        private CreateSessionResult() {}

        public abstract T Accept<T>(ICreateSessionResultVisitor<T> visitor);

        public sealed class Created : CreateSessionResult
        {
            public Created(UserSession userSession, CookieJar cookies)
            {
                UserSession = userSession;
                Cookies = cookies;
            }

            public UserSession UserSession { get; }
            public CookieJar Cookies { get; }

            public override T Accept<T>(ICreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class BadRequest : CreateSessionResult
        {
            public BadRequest(string serviceDeskReference) => ServiceDeskReference = serviceDeskReference;

            public string ServiceDeskReference { get; }

            public override T Accept<T>(ICreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class OdsCodeNotSupported : CreateSessionResult
        {
            public OdsCodeNotSupported(string serviceDeskReference) => ServiceDeskReference = serviceDeskReference;

            public string ServiceDeskReference { get; }

            public override T Accept<T>(ICreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class OdsCodeNotFound : CreateSessionResult
        {
            public OdsCodeNotFound(string serviceDeskReference) => ServiceDeskReference = serviceDeskReference;

            public string ServiceDeskReference { get; }

            public override T Accept<T>(ICreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class NoNhsNumber : CreateSessionResult
        {
            public NoNhsNumber(string serviceDeskReference) => ServiceDeskReference = serviceDeskReference;

            public string ServiceDeskReference { get; }

            public override T Accept<T>(ICreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class FailedAgeRequirement : CreateSessionResult
        {
            public FailedAgeRequirement(string serviceDeskReference) => ServiceDeskReference = serviceDeskReference;

            public string ServiceDeskReference { get; }

            public override T Accept<T>(ICreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class BadResponseFromUpstreamSystem : CreateSessionResult
        {
            public BadResponseFromUpstreamSystem(string serviceDeskReference) => ServiceDeskReference = serviceDeskReference;

            public string ServiceDeskReference { get; }

            public override T Accept<T>(ICreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class UpstreamSystemTimeout : CreateSessionResult
        {
            public UpstreamSystemTimeout(string serviceDeskReference) => ServiceDeskReference = serviceDeskReference;

            public string ServiceDeskReference { get; }

            public override T Accept<T>(ICreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class InternalServerError : CreateSessionResult
        {
            public InternalServerError(string serviceDeskReference) => ServiceDeskReference = serviceDeskReference;

            public string ServiceDeskReference { get; }

            public override T Accept<T>(ICreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class Failed : CreateSessionResult
        {
            public override T Accept<T>(ICreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}