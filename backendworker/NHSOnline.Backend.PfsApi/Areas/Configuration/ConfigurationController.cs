using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.Areas.Configuration.Models;
using NHSOnline.Backend.PfsApi.Configuration;
using NHSOnline.Backend.PfsApi.Devices;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.Configuration
{
    [ApiVersion("1")]
    [ApiVersionRoute("configuration")]
    public class ConfigurationController : Controller
    {
        private readonly ILogger<ConfigurationController> _logger;
        private readonly ISupportedDeviceService _supportedDeviceService;
        private readonly IConfigurationService _configurationService;

        public ConfigurationController(ILogger<ConfigurationController> logger,
            ISupportedDeviceService supportedDeviceService,
            IConfigurationService configurationService)
        {
            _logger = logger;
            _supportedDeviceService = supportedDeviceService;
            _configurationService = configurationService;
        }
        
        [HttpGet, AllowAnonymous]
        public IActionResult Get([FromQuery] GetConfigurationQueryParameters deviceDetails)
        {
            try
            {
                _logger.LogEnter();

                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }

                if (string.IsNullOrEmpty(deviceDetails?.DeviceName) ||
                    string.IsNullOrEmpty(deviceDetails.NativeAppVersion))
                {
                    _logger.LogDebug($"Request does not contain {nameof(deviceDetails.DeviceName)} or {nameof(deviceDetails.NativeAppVersion)}");
                    return BadRequest();
                }

                var device = new DeviceDetails
                {
                    Name = deviceDetails.DeviceName,
                    NativeAppVersion = deviceDetails.NativeAppVersion,
                };

                var isDeviceSupported = _supportedDeviceService.IsDeviceSupported(device);
                return isDeviceSupported.Accept(new GetConfigurationResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [ApiVersion("2")]
        [HttpGet, AllowAnonymous]
        public IActionResult GetV2()
        {
            try
            {
                _logger.LogEnter();
                
                var result = _configurationService.GetConfiguration();
                return Ok(result.Response);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}