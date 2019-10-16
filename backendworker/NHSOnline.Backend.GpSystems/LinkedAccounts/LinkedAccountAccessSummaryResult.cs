using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;

namespace NHSOnline.Backend.GpSystems.LinkedAccounts
{
    public abstract class LinkedAccountAccessSummaryResult
    {
        public abstract T Accept<T>(ILinkedAccountAccessSummaryResultVisitor<T> visitor);

        public class Success : LinkedAccountAccessSummaryResult
        {
            public GetLinkedAccountAccessSummaryResponse Response { get; }

            public Success(GetLinkedAccountAccessSummaryResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(ILinkedAccountAccessSummaryResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotFound : LinkedAccountAccessSummaryResult
        {
            public override T Accept<T>(ILinkedAccountAccessSummaryResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : LinkedAccountAccessSummaryResult
        {
            public override T Accept<T>(ILinkedAccountAccessSummaryResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
