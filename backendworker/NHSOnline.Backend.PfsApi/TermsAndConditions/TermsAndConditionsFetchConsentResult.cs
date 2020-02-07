using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public abstract class TermsAndConditionsFetchConsentResult
    {
        public abstract T Accept<T>(ITermsAndConditionsFetchConsentResultVisitor<T> visitor);

        public class Success : TermsAndConditionsFetchConsentResult
        {
            public ConsentResponse Response { get; }

            public Success(ConsentResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(ITermsAndConditionsFetchConsentResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NoConsentFound : TermsAndConditionsFetchConsentResult
        {
            public ConsentResponse Response { get; }


            public NoConsentFound(ConsentResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(ITermsAndConditionsFetchConsentResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : TermsAndConditionsFetchConsentResult
        {
            public override T Accept<T>(ITermsAndConditionsFetchConsentResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}