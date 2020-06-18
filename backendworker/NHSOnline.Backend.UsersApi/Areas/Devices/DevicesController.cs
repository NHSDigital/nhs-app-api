using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Registrations;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    [Route("api/users/devices")]
    public class DevicesController : Controller
    {
        private readonly IRegistrationService _registrationService;
        private readonly ILogger<DevicesController> _logger;

        public DevicesController(IRegistrationService registrationService, ILogger<DevicesController> logger)
        {
            _logger = logger;
            _registrationService = registrationService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string devicePns)
        {
            try
            {
                _logger.LogEnter();

                if (!IsDevicePnsValid(devicePns))
                {
                    return BadRequest();
                }

                var accessToken = HttpContext.GetAccessToken(_logger);

                try
                {
                    var result = await _registrationService.GetRegistration(devicePns, accessToken);
                    return result.Accept(new RegistrationExistsResultVisitor());
                }
                catch (Exception e)
                {
                    _logger.LogError($"Retrieving user device registration failed with exception: {e}");
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string devicePns)
        {
            try
            {
                _logger.LogEnter();

                if (!IsDevicePnsValid(devicePns))
                {
                    return BadRequest();
                }

                var accessToken = HttpContext.GetAccessToken(_logger);

                try
                {
                    var result = await _registrationService.DeleteRegistration(devicePns, accessToken);
                    return result.Accept(new DeleteRegistrationResultVisitor());
                }
                catch (Exception e)
                {
                    _logger.LogError($"Registration Deletion failed with exception: {e}");
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegisterDeviceRequest model)
        {
            try
            {
                _logger.LogEnter();

                if (!IsRegisterDeviceRequestValid(model))
                {
                    return BadRequest();
                }

                var accessToken = HttpContext.GetAccessToken(_logger);

                try
                {
                    var registrationResult = await _registrationService.CreateRegistration(model, accessToken);
                    return registrationResult.Accept(new RegisterDeviceResultVisitor(model));
                }
                catch (Exception e)
                {
                    _logger.LogError($"Registration Creation failed with exception: {e}");
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private bool IsDevicePnsValid(string devicePns)
        {
            return new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(devicePns, nameof(devicePns))
                .IsValid();
        }

        private bool IsRegisterDeviceRequestValid(RegisterDeviceRequest request)
        {
            return new ValidateAndLog(_logger)
                .IsNotNull(request, nameof(request))
                .IsNotNullOrWhitespace(request?.DevicePns, nameof(request.DevicePns))
                .IsNotNull(request?.DeviceType, nameof(request.DeviceType))
                .IsValid();
        }
    }
}