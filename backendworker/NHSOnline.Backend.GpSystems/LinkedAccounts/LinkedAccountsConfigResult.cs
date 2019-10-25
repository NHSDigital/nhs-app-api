using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;

namespace NHSOnline.Backend.GpSystems.LinkedAccounts
{
    public abstract class LinkedAccountsConfigResult
    {
        public abstract T Accept<T>(ILinkedAccountsConfigResultVisitor<T> visitor);

        public class Success : LinkedAccountsConfigResult
        {
            public LinkedAccountsConfigResponse Response { get; }

            public Success(LinkedAccountsConfigResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(ILinkedAccountsConfigResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }        
    }
}