using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity.Models;

namespace NHSOnline.Backend.PfsApi.AssertedLoginIdentity
{
    public abstract class CreateJwtResult : IAuditedResult
    {
        public abstract T Accept<T>(ICreateJwtResultVisitor<T> visitor);

        public class Success : CreateJwtResult
        {
            public CreateJwtResponse Response { get; }

            public Success(CreateJwtResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(ICreateJwtResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }

            public override string Details => "AssertedLoginIdentity Token creation succeeded.";
        }

        public class InternalServerError : CreateJwtResult
        {
            public override T Accept<T>(ICreateJwtResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }

            public override string Details => "AssertedLoginIdentity Token creation failed.";
        }

        public abstract string Details { get; }
    }
}