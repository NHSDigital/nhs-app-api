namespace NHSOnline.Backend.Worker.TermsAndConditions
{
    public abstract class TermsAndConditionsRecordConsentResult
    {       
        public abstract T Accept<T>(ITermsAndConditionsRecordConsentResultVisitor<T> visitor);

        public class InitialConsentRecorded : TermsAndConditionsRecordConsentResult
        {
            public override T Accept<T>(ITermsAndConditionsRecordConsentResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }    
        }
        
        public class UpdateConsentRecorded : TermsAndConditionsRecordConsentResult
        {
            public override T Accept<T>(ITermsAndConditionsRecordConsentResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }    
        }

        public class FailureToRecordConsent : TermsAndConditionsRecordConsentResult
        {
            public override T Accept<T>(ITermsAndConditionsRecordConsentResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}