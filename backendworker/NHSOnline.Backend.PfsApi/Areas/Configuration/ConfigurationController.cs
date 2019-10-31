using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.Areas.Configuration.Models;
using NHSOnline.Backend.PfsApi.Devices;

namespace NHSOnline.Backend.PfsApi.Areas.Configuration
{
    [Route("configuration")]
    public class ConfigurationController : Controller
    {
        private readonly ILogger<ConfigurationController> _logger;
        private readonly ISupportedDeviceService _supportedDeviceService;

        public ConfigurationController(ILogger<ConfigurationController> logger, ISupportedDeviceService supportedDeviceService)
        {
            _logger = logger;
            _supportedDeviceService = supportedDeviceService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get([FromQuery] GetConfigurationQueryParameters deviceDetails)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            
            if (string.IsNullOrEmpty(deviceDetails?.DeviceName) || string.IsNullOrEmpty(deviceDetails?.NativeAppVersion))
            {
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
    }
}