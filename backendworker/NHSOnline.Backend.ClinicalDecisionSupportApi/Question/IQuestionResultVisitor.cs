using NHSOnline.Backend.ClinicalDecisionSupportApi.Question.Models;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Question
{
    public interface IQuestionResultVisitor<out T>
    {
        T Visit(QuestionResult.SuccessfullyRetrieved result);
        T Visit(QuestionResult.Unsuccessful result);
        T Visit(QuestionResult.BadRequest result);
    }
}