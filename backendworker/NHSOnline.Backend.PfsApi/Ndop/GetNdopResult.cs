using NHSOnline.Backend.PfsApi.Ndop.Models;

namespace NHSOnline.Backend.PfsApi.Ndop
{
    public abstract class GetNdopResult
    {
        public abstract T Accept<T>(INdopResultVisitor<T> visitor);

        public class Success : GetNdopResult
        {
            public NdopResponse Response { get; }

            public Success(NdopResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(INdopResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : GetNdopResult
        {
            public override T Accept<T>(INdopResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}