using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;

namespace NHSOnline.Backend.GpSystems.LinkedAccounts
{
    public abstract class LinkedAccountsResult
    {
        public abstract T Accept<T>(ILinkedAccountsResultVisitor<T> visitor);

        public class Success : LinkedAccountsResult
        {
            public IEnumerable<LinkedAccount> ValidAccounts { get; } = Enumerable.Empty<LinkedAccount>();

            public bool HasAnyProxyInfoBeenUpdatedInSession { get; }

            public Success(IEnumerable<LinkedAccount> linkedAccounts, bool hasAnyProxyInfoBeenUpdatedInSession)
            {
                ValidAccounts = linkedAccounts;
                HasAnyProxyInfoBeenUpdatedInSession = hasAnyProxyInfoBeenUpdatedInSession;
            }

            public override T Accept<T>(ILinkedAccountsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
