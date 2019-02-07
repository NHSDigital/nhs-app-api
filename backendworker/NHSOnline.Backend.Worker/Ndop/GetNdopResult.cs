using NHSOnline.Backend.Worker.Ndop.Models;

namespace NHSOnline.Backend.Worker.Ndop
{
    public abstract class GetNdopResult
    {
        public abstract T Accept<T>(INdopResultVisitor<T> visitor);

        public class SuccessfullyRetrieved : GetNdopResult
        {
            public NdopResponse Response { get; }

            public SuccessfullyRetrieved(NdopResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(INdopResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Unsuccessful : GetNdopResult
        {
            public override T Accept<T>(INdopResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}