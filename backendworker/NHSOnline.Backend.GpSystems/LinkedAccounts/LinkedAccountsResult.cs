using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;

namespace NHSOnline.Backend.GpSystems.LinkedAccounts
{
    public abstract class LinkedAccountsResult
    {
        public abstract T Accept<T>(ILinkedAccountsResultVisitor<T> visitor);

        public class Success : LinkedAccountsResult
        {
            public GetLinkedAccountsResponse Response { get; }

            public Success(GetLinkedAccountsResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(ILinkedAccountsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
