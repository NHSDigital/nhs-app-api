using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Controllers
{
    [ApiController]
    [Route("api/odscodes")]
    public class OdsCodesController : ControllerBase
    {
        private readonly ILogger<OdsCodesController> _logger;
        private readonly IJourneyRepository _journeyRepository;

        public OdsCodesController(ILogger<OdsCodesController> logger, IJourneyRepository journeyRepository)
        {
            _logger = logger;
            _journeyRepository = journeyRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            try
            {
                _logger.LogEnter();

                return new OkObjectResult(_journeyRepository.GetOdsCodes());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve ODS codes");

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
