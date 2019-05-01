using NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models.Fhir;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models
{
    public abstract class QuestionnaireResult
    {
        public abstract T Accept<T>(IQuestionnaireResultVisitor<T> visitor);

        public class SuccessfullyRetrieved : QuestionnaireResult
        {
            public FhirQuestionnaire Response { get; }

            public SuccessfullyRetrieved(FhirQuestionnaire response)
            {
                Response = response;
            }

            public override T Accept<T>(IQuestionnaireResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Unsuccessful : QuestionnaireResult
        {
            public override T Accept<T>(IQuestionnaireResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadRequest : QuestionnaireResult
        {
            public override T Accept<T>(IQuestionnaireResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}