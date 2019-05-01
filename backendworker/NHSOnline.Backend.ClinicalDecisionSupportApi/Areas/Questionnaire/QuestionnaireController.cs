using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models;
using NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Areas.Questionnaire
{
    [ApiController]
    [Route("fhir/Questionnaire/{id}")]
    public class QuestionnaireController : Controller
    {
        private readonly IQuestionnaireService _service;
        private readonly ILogger<QuestionnaireController> _logger;
        
        public QuestionnaireController(IQuestionnaireService service, ILoggerFactory loggerFactory)
        {
            _service = service;
            _logger = loggerFactory.CreateLogger<QuestionnaireController>();
        }

        // TODO: NHSO-5321 disallow anonymous
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetQuestionnaireById([FromRoute] string id)
        {
            try
            {
                _logger.LogEnter();

                var visitor = new QuestionnaireResultVisitor();

                if (string.IsNullOrWhiteSpace(id))
                {
                    _logger.LogError("Missing Questionnaire id in route");
                    
                    var badRequestResult = new QuestionnaireResult.BadRequest();

                    return await Task.FromResult(badRequestResult.Accept(visitor));
                }
                
                _logger.LogInformation($"Retrieving Questionnaire for id {id}");

                var result = await _service.GetQuestionnaireById(id);
               
                return result.Accept(visitor);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}