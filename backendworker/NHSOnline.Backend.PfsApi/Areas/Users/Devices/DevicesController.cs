using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Users.Areas.Devices;
using NHSOnline.Backend.Users.Areas.Devices.Models;
using NHSOnline.Backend.Users.Registrations;

namespace NHSOnline.Backend.PfsApi.Areas.Users.Devices
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DevicesController : Controller
    {
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly IRegistrationService _registrationService;
        private readonly ILogger<DevicesController> _logger;
        private readonly IMetricLogger<AccessTokenMetricContext> _metricLogger;
        private readonly INotificationsDecisionAuditService _notificationsDecisionAuditService;

        public DevicesController(
            IRegistrationService registrationService,
            ILogger<DevicesController> logger,
            INotificationsDecisionAuditService notificationsDecisionAuditService,
            IMetricLogger<AccessTokenMetricContext> metricLogger,
            IAccessTokenProvider accessTokenProvider)
        {
            _logger = logger;
            _registrationService = registrationService;
            _metricLogger = metricLogger;
            _notificationsDecisionAuditService = notificationsDecisionAuditService;
            _accessTokenProvider = accessTokenProvider;
        }

        [ApiVersionRoute("api/users/me/devices")]
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

                var accessToken = _accessTokenProvider.AccessToken;

                try
                {
                    var result = await _registrationService.GetRegistration(devicePns, accessToken);
                    return result.Accept(new RegistrationExistsResultVisitor());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Retrieving user device registration failed with exception");
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [ApiVersionRoute("api/users/me/devices")]
        [HttpDelete]
        [UserProfile]
        public async Task<IActionResult> Delete([FromQuery] string devicePns)
        {
            try
            {
                _logger.LogEnter();

                if (!IsDevicePnsValid(devicePns))
                {
                    return BadRequest();
                }

                var accessToken = _accessTokenProvider.AccessToken;

                try
                {
                    var result = await _registrationService.DeleteRegistration(devicePns, accessToken);
                    return await result.Accept(new DeleteRegistrationResultVisitor(_metricLogger));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Registration Deletion failed with exception");
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [ApiVersionRoute("api/users/me/devices")]
        [HttpPost]
        [UserProfile]
        public async Task<IActionResult> Post([FromBody] RegisterDeviceRequest model)
        {
            try
            {
                _logger.LogEnter();

                if (!IsRegisterDeviceRequestValid(model))
                {
                    return BadRequest();
                }

                var accessToken = _accessTokenProvider.AccessToken;

                try
                {
                    var registrationResult = await _registrationService.CreateRegistration(model, accessToken);
                    return await registrationResult.Accept(new RegisterDeviceResultVisitor(model, _metricLogger));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Registration Creation failed with exception");
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