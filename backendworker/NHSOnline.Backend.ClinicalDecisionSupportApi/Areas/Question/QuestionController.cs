using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ClinicalDecisionSupportApi.Question.Models;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Areas.Question
{
    [ApiController]
    [Route("question")]
    public class QuestionController : Controller
    {
        private readonly ILogger<QuestionController> _logger;
        
        public QuestionController(
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<QuestionController>();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetQuestionForType([FromQuery]string questionType)
        {   
            try
            {
                _logger.LogEnter();

                var visitor = new QuestionResultVisitor();

                if (string.IsNullOrEmpty(questionType))
                {
                    _logger.LogError("Missing question type");
                    
                    var badRequestResult = new QuestionResult.BadRequest();
                    return await Task.FromResult(badRequestResult.Accept(visitor));
                }
                
                _logger.LogInformation($"Retrieving question for type: {questionType}");

                var successfulResult = new QuestionResult.SuccessfullyRetrieved(new QuestionResponse
                {
                    Question = $"What is your favourite {questionType} question?"
                });
                
                return await Task.FromResult(successfulResult.Accept(visitor));
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}