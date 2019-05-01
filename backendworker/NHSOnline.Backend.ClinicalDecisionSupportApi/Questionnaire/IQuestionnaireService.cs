using System.Threading.Tasks;
using NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire
{
    public interface IQuestionnaireService
    {
        Task<QuestionnaireResult> GetQuestionnaireById(string id);
    }
}