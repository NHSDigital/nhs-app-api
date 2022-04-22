using NHSOnline.Backend.PfsApi.NHSApim.Models;

namespace NHSOnline.Backend.PfsApi.NHSApim
{
    public abstract class GetAuthTokenResult
    {
        public abstract T Accept<T>(INhsApimResultVisitor<T> visitor);

        public class Success : GetAuthTokenResult
        {
            public ApimAccessToken Response { get; }

            public Success(ApimAccessToken response)
            {
                Response = response;
            }

            public override T Accept<T>(INhsApimResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class BadRequest : GetAuthTokenResult
        {
            public ApimAccessToken Response { get; }

            public BadRequest(ApimAccessToken response)
            {
                Response = response;
            }

            public override T Accept<T>(INhsApimResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class Forbidden : GetAuthTokenResult
        {
            public ApimAccessToken Response { get; }

            public Forbidden(ApimAccessToken response)
            {
                Response = response;
            }

            public override T Accept<T>(INhsApimResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}