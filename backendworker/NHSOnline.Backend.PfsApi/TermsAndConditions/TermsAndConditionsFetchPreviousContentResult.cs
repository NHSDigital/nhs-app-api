using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public abstract class TermsAndConditionsFetchPreviousConsentResult
    {
        public abstract T Accept<T>(ITermsAndConditionsFetchPreviousConsentResultVisitor<T> visitor);

        public class Success : TermsAndConditionsFetchPreviousConsentResult
        {
            public ConsentResponse Response { get; }

            public Success(ConsentResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(ITermsAndConditionsFetchPreviousConsentResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : TermsAndConditionsFetchPreviousConsentResult
        {
            public override T Accept<T>(ITermsAndConditionsFetchPreviousConsentResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}