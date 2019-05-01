using System.Threading.Tasks;
using NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models;
using NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models.Fhir;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire
{
    public interface IQuestionnaireClient
    {
        Task<FhirApiQuestionnaireResponse<FhirQuestionnaire>> GetQuestionnaireById(string id);
    }
}