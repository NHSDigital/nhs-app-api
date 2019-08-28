using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    [Route("api/users/devices")]
    public class DevicesController : Controller
    {
        private readonly INotificationRegistrationService _notificationRegistrationService;
        private readonly IDeviceRepositoryService _deviceServiceRepository;
        private readonly ILogger<DevicesController> _logger;

        public DevicesController(INotificationRegistrationService notificationRegistrationService,
            IDeviceRepositoryService deviceServiceRepository,
            ILogger<DevicesController> logger)
        {
            _notificationRegistrationService = notificationRegistrationService;
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

                var accessToken = HttpContext.GetAccessToken(_logger);

                try
                {
                    var registrationResponse = await _notificationRegistrationService.Register(model, accessToken);

                    if (registrationResponse is RegistrationResult.Success successRegistrationResult)
                    {
                        var deviceRepositoryResult = await _deviceServiceRepository.Create(successRegistrationResult.Response, model, accessToken);
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