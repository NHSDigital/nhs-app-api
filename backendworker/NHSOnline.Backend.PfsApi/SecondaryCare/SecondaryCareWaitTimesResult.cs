using NHSOnline.Backend.PfsApi.Areas.SecondaryCare;
using NHSOnline.Backend.PfsApi.SecondaryCare.Models;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public abstract class SecondaryCareWaitTimesResult
    {
        public abstract T Accept<T>(ISecondaryCareWaitTimesResultVisitor<T> visitor);

        public class Success : SecondaryCareWaitTimesResult
        {
            public WaitTimesResponse Response { get; }

            public Success(WaitTimesResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(ISecondaryCareWaitTimesResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class BadGateway : SecondaryCareWaitTimesResult
        {
            public override T Accept<T>(ISecondaryCareWaitTimesResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class Timeout : SecondaryCareWaitTimesResult
        {
            public override T Accept<T>(ISecondaryCareWaitTimesResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class NotEnabled : SecondaryCareWaitTimesResult
        {
            public override T Accept<T>(ISecondaryCareWaitTimesResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}