namespace NHSOnline.Backend.Auth.CitizenId
{
    public abstract class RefreshAccessTokenResult
    {
        public abstract T Accept<T>(IRefreshTokenResultVisitor<T> visitor);
        
        public class Success : RefreshAccessTokenResult
        {
            public Success(string accessToken)
            {
                AccessToken = accessToken;
            }
            
            public string AccessToken { get; }
            
            public override T Accept<T>(IRefreshTokenResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : RefreshAccessTokenResult
        {
            public override T Accept<T>(IRefreshTokenResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : RefreshAccessTokenResult
        {
            public override T Accept<T>(IRefreshTokenResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}