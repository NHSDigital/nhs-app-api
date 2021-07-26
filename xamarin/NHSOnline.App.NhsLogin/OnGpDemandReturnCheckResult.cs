using System;

namespace NHSOnline.App.NhsLogin
{
    public abstract class OnGpDemandReturnCheckResult
    {
        private OnGpDemandReturnCheckResult(){}

        public abstract T Accept<T>(IOnGpDemandReturnCheckResultVisitor<T> visitor);

        public sealed class Complete: OnGpDemandReturnCheckResult
        {
            public Complete(Uri redirectUri)
            {
                RedirectUri = redirectUri;
            }

            public Uri RedirectUri { get; }

            public override T Accept<T>(IOnGpDemandReturnCheckResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}