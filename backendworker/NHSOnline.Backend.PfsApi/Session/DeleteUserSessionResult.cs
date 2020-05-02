using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Areas.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    public abstract class DeleteUserSessionResult : IAuditedResult
    {
        private DeleteUserSessionResult() { }

        public abstract string Details { get; }
        internal abstract T Accept<T>(IDeleteUserSessionResultVisitor<T> visitor);

        internal sealed class Success : DeleteUserSessionResult
        {
            public override string Details => "Session successfully deleted";
            internal override T Accept<T>(IDeleteUserSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class Failure : DeleteUserSessionResult
        {
            public override string Details => "Delete session failed";
            internal override T Accept<T>(IDeleteUserSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}