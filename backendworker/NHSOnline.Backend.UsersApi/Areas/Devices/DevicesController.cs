using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Azure;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    [Route("api/users/devices")]
    public class DevicesController : Controller
    {
        private readonly IAzureNotificationHubService _hubService;
        private readonly IDeviceRepositoryService _deviceServiceRepository;
        private readonly ILogger<DevicesController> _logger;

        public DevicesController(IAzureNotificationHubService hubService,
            IDeviceRepositoryService deviceServiceRepository,
            ILogger<DevicesController> logger)
        {
            _hubService = hubService;
            _deviceServiceRepository = deviceServiceRepository;
            _logger = logger;
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

                try
                {
                    var registrationResponse = await _hubService.Register(model);

                    if (registrationResponse is RegistrationResult.Success successRegistrationResult)
                    {
                        var deviceRepositoryResult = await _deviceServiceRepository.Create(successRegistrationResult.Response, model);
                        return deviceRepositoryResult.Accept(new DeviceRepositoryResultVisitor(model, _logger));
                    }

                    return registrationResponse.Accept(new RegistrationResultVisitor(_logger));
                }
                catch (Exception e)
                {
                    _logger.LogError($"User Creation failed with exception: {e}");
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
            }
            finally
            {
                _logger.LogExit();
            }
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