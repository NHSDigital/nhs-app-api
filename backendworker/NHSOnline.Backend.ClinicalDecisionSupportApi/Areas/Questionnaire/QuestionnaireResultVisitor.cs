using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire;
using NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Areas.Questionnaire
{
    public class QuestionnaireResultVisitor : IQuestionnaireResultVisitor<IActionResult>
    {
        public IActionResult Visit(QuestionnaireResult.SuccessfullyRetrieved result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(QuestionnaireResult.Unsuccessful result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        
        public IActionResult Visit(QuestionnaireResult.BadRequest result)
        {
            return new BadRequestResult();
        }
    }
}