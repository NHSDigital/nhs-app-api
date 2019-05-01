using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.ClinicalDecisionSupportApi.Question;
using NHSOnline.Backend.ClinicalDecisionSupportApi.Question.Models;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Areas.Question
{
    public class QuestionResultVisitor : IQuestionResultVisitor<IActionResult>
    {
        public IActionResult Visit(QuestionResult.SuccessfullyRetrieved result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(QuestionResult.Unsuccessful result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        
        public IActionResult Visit(QuestionResult.BadRequest result)
        {
            return new BadRequestResult();
        }
    }
}