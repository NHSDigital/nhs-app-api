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

        public class Unauthorized : GetAuthTokenResult
        {
            public ApimAccessToken Response { get; }

            public Unauthorized(ApimAccessToken response)
            {
                Response = response;
            }

            public override T Accept<T>(INhsApimResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}