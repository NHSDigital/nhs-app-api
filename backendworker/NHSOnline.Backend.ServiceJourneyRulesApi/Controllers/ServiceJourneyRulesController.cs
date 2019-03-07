using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
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
        public ActionResult<ServiceJourneyRulesResponse> Get([FromQuery]string odsCode)
        {   
            try
            {
                _logger.LogEnter();

                _logger.LogInformation($"Retrieving Service Journey Rules for ods code: {odsCode}");
                
                var result = _serviceJourneyRulesService.GetServiceJourneyRulesForOdsCode(odsCode);
                return result;
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}