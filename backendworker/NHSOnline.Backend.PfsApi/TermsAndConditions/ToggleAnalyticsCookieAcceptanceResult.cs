using NHSOnline.Backend.Auditing;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public abstract class ToggleAnalyticsCookieAcceptanceResult: IAuditedResult
    {
        public abstract T Accept<T>(IToggleAnalyticsCookieAcceptanceVisitor<T> visitor);

        public abstract string Details { get; }

        public class Success : ToggleAnalyticsCookieAcceptanceResult
        {
            public override T Accept<T>(IToggleAnalyticsCookieAcceptanceVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }

            public override string Details => "Analytics Cookie Consent toggled successfully";
        }
        
        public class Failure : ToggleAnalyticsCookieAcceptanceResult
        {
            public override T Accept<T>(IToggleAnalyticsCookieAcceptanceVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }

            public override string Details => "Failed to toggle analytics cookie";
        }
    }
}