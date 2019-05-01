using NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire
{
    public interface IQuestionnaireResultVisitor<out T>
    {
        T Visit(QuestionnaireResult.SuccessfullyRetrieved result);
        T Visit(QuestionnaireResult.Unsuccessful result);
        T Visit(QuestionnaireResult.BadRequest result);
    }
}