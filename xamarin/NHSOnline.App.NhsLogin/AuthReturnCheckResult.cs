using System;

namespace NHSOnline.App.NhsLogin
{
    public abstract class AuthReturnCheckResult
    {
        private AuthReturnCheckResult(){}

        public abstract T Accept<T>(IAuthReturnCheckResultVisitor<T> visitor);

        public sealed class Authorised: AuthReturnCheckResult
        {
            public Authorised(Uri redirectUri, string authCode)
            {
                RedirectUri = redirectUri;
                AuthCode = authCode;
            }

            public Uri RedirectUri { get; }
            public string AuthCode { get; }

            public override T Accept<T>(IAuthReturnCheckResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class Failed : AuthReturnCheckResult
        {
            public override T Accept<T>(IAuthReturnCheckResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}