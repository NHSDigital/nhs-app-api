using System.Collections.Generic;

namespace NHSOnline.App.NhsLogin
{
    public abstract class OnDemandGpReturnCheckResult
    {
        private OnDemandGpReturnCheckResult(){}

        public abstract T Accept<T>(IOnDemandGpReturnCheckResultVisitor<T> visitor);

        public sealed class Complete: OnDemandGpReturnCheckResult
        {
            public Dictionary<string, string> Parameters { get; }

            public Complete(Dictionary<string, string> parameters)
            {
                Parameters = parameters;
            }

            public override T Accept<T>(IOnDemandGpReturnCheckResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}