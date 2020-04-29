using NHSOnline.Backend.Auditing;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public abstract class TermsAndConditionsRecordConsentResult: IAuditedResult
    {
        public abstract T Accept<T>(ITermsAndConditionsRecordConsentResultVisitor<T> visitor);

        public abstract string Details { get; }

        public class InitialConsentRecorded : TermsAndConditionsRecordConsentResult
        {
            public override T Accept<T>(ITermsAndConditionsRecordConsentResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }

            public override string Details => "Initial Consent Successfully recorded";
        }

        public class UpdateConsentRecorded : TermsAndConditionsRecordConsentResult
        {
            public override T Accept<T>(ITermsAndConditionsRecordConsentResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }

            public override string Details => "Updated Consent Successfully recorded";
        }

        public class InternalServerError : TermsAndConditionsRecordConsentResult
        {
            public override T Accept<T>(ITermsAndConditionsRecordConsentResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }

            public override string Details => "Failed to record";
        }
    }
}