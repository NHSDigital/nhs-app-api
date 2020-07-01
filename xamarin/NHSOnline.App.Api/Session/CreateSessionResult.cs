namespace NHSOnline.App.Api.Session
{
    public abstract class CreateSessionResult
    {
        private CreateSessionResult() {}

        public abstract T Accept<T>(ICreateSessionResultVisitor<T> visitor);

        public sealed class Created : CreateSessionResult
        {
            public Created(UserSession userSession) => UserSession = userSession;

            public UserSession UserSession { get; }

            public override T Accept<T>(ICreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class Failed : CreateSessionResult
        {
            public override T Accept<T>(ICreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}