namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public abstract class ToggleAnalyticsCookieAcceptanceResult
    {
        public abstract T Accept<T>(IToggleAnalyticsCookieAcceptanceVisitor<T> visitor);
        
        public class Success : ToggleAnalyticsCookieAcceptanceResult
        {
            public override T Accept<T>(IToggleAnalyticsCookieAcceptanceVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }    
        }
        
        public class Failure : ToggleAnalyticsCookieAcceptanceResult
        {
            public override T Accept<T>(IToggleAnalyticsCookieAcceptanceVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }    
        }
    }
}