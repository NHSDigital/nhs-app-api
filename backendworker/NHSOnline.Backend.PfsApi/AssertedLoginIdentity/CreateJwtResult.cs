using NHSOnline.Backend.PfsApi.AssertedLoginIdentity.Models;

namespace NHSOnline.Backend.PfsApi.AssertedLoginIdentity
{
    public abstract class CreateJwtResult
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
        }

        public class InternalServerError : CreateJwtResult
        {
            public override T Accept<T>(ICreateJwtResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}