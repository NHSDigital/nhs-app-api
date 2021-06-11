using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.Configuration;
using NHSOnline.Backend.PfsApi.KnownServices;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.KnownServices
{
    [ApiVersion("3")]
    [ApiVersionRoute("known-services")]
    public class KnownServicesController : Controller
    {
        private readonly ILogger<KnownServicesController> _logger;
        private readonly IKnownServicesService _knownServicesService;

        public KnownServicesController(ILogger<KnownServicesController> logger,
            IKnownServicesService knownServicesService)
        {
            _logger = logger;
            _knownServicesService = knownServicesService;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Get()
        {
            try
            {
                _logger.LogEnter();

                var result = _knownServicesService.GetConfiguration();
                return Ok(result.V3Response);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}