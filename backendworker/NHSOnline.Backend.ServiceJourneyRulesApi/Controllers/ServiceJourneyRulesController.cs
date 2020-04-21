using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.Service;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Controllers
{
    [ApiController]
    [Route("api/servicejourneyrules")]
    public class ServiceJourneyRulesController : ControllerBase
    {
        private readonly ILogger<ServiceJourneyRulesController> _logger;
        private readonly IServiceJourneyRulesService _serviceJourneyRulesService;

        public ServiceJourneyRulesController(
            ILoggerFactory loggerFactory,
            IServiceJourneyRulesService serviceJourneyRulesService)
        {
            _serviceJourneyRulesService = serviceJourneyRulesService;
            _logger = loggerFactory.CreateLogger<ServiceJourneyRulesController>();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get([FromQuery]string odsCode)
        {
            try
            {
                _logger.LogEnter();

                if (string.IsNullOrWhiteSpace(odsCode))
                {
                    _logger.LogError("Ods code not provided");
                    return BadRequest();
                }

                return RetrieveSjrRulesForOds(odsCode);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpGet("no-ods")]
        [AllowAnonymous]
        public IActionResult Get()
        {
            try
            {
                _logger.LogEnter();

                _logger.LogInformation($"Retrieving Service Journey Rules for no ods code");

                return RetrieveSjrRulesForOds(Constants.OdsCode.None);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private IActionResult RetrieveSjrRulesForOds(string odsCode)
        {
            var result = _serviceJourneyRulesService.GetServiceJourneyRulesForOdsCode(odsCode);

            if (result?.Journeys == null)
            {
                return NotFound();
            }

            return new OkObjectResult(result);
        }
    }
}