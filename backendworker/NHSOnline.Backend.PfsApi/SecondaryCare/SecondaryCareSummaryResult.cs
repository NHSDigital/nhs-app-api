using NHSOnline.Backend.PfsApi.Areas.SecondaryCare;
using NHSOnline.Backend.PfsApi.SecondaryCare.Models;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public abstract class SecondaryCareSummaryResult
    {
        public abstract T Accept<T>(ISecondaryCareSummaryResultVisitor<T> visitor);

        public class Success : SecondaryCareSummaryResult
        {
            public SummaryResponse Response { get; }

            public Success(SummaryResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(ISecondaryCareSummaryResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class BadGateway : SecondaryCareSummaryResult
        {
            public override T Accept<T>(ISecondaryCareSummaryResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}