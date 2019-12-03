using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;

namespace NHSOnline.Backend.GpSystems.LinkedAccounts
{
    public abstract class LinkedAccountsResult
    {
        public abstract T Accept<T>(ILinkedAccountsResultVisitor<T> visitor);

        public class Success : LinkedAccountsResult
        {
            public LinkedAccountsBreakdownSummary LinkedAccountsBreakdown { get; }

            public bool HasAnyProxyInfoBeenUpdatedInSession { get; }

            public Success(LinkedAccountsBreakdownSummary linkedAccountsBreakdown, bool hasAnyProxyInfoBeenUpdatedInSession)
            {
                LinkedAccountsBreakdown = linkedAccountsBreakdown;
                HasAnyProxyInfoBeenUpdatedInSession = hasAnyProxyInfoBeenUpdatedInSession;
            }

            public override T Accept<T>(ILinkedAccountsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
