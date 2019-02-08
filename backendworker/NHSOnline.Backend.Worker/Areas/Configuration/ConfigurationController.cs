using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Configuration.Models;
using NHSOnline.Backend.Worker.Support.Devices;

namespace NHSOnline.Backend.Worker.Areas.Configuration
{
    [Route("configuration"), PfsSecurityMode]
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