using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.Auth.CitizenId.Models;
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
        private readonly INotificationService _notificationService;
        private readonly IDeviceRepositoryService _deviceRepositoryService;
        private readonly IAuditor _auditor;
        private readonly ILogger<DevicesController> _logger;

        public DevicesController
        (
            INotificationService notificationService,
            IDeviceRepositoryService deviceRepositoryService,
            ILogger<DevicesController> logger,
            IAuditor auditor
        )
        {
            _notificationService = notificationService;
            _deviceRepositoryService = deviceRepositoryService;
            _logger = logger;
            _auditor = auditor;
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

                await _auditor.AuditSecureTokenEvent(accessToken, Supplier.Microsoft,
                    AuditingOperations.UsersDeviceGetAuditTypeRequest,
                    "Attempting to get user notification registration");

                try
                {
                    var searchDeviceResult = await _deviceRepositoryService.Find(devicePns, accessToken);

                    if (!(searchDeviceResult is SearchDeviceResult.Found foundDeviceResult))
                    {
                        await searchDeviceResult.Accept(
                            new SearchDeviceAuditingVisitor(
                                _logger,
                                _auditor,
                                accessToken,
                                AuditingOperations.UsersDeviceGetAuditTypeResponse));
                        return searchDeviceResult.Accept(new SearchDeviceResultVisitor());
                    }

                    return await VerifyRegistration(
                        foundDeviceResult,
                        accessToken);
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

                await _auditor.AuditSecureTokenEvent(accessToken, Supplier.Microsoft,
                    AuditingOperations.UsersDeviceDeleteAuditTypeRequest,
                    "Attempting to delete user notification registration");

                try
                {
                    var searchDeviceResult = await _deviceRepositoryService.Find(devicePns, accessToken);
                    if (!(searchDeviceResult is SearchDeviceResult.Found foundDeviceResult))
                    {
                        await searchDeviceResult.Accept(
                            new SearchDeviceAuditingVisitor(_logger,
                                _auditor,
                                accessToken,
                                AuditingOperations.UsersDeviceDeleteAuditTypeResponse));
                        return searchDeviceResult.Accept(new SearchDeviceResultVisitor());
                    }

                    var userDevice = foundDeviceResult.UserDevice;

                    var deleteRegistrationResult = await _notificationService.Delete(userDevice.RegistrationId);
                    if (!(deleteRegistrationResult is DeleteRegistrationResult.Success))
                    {
                        await deleteRegistrationResult.Accept(
                            new DeleteRegistrationAuditingVisitor(_logger, _auditor, accessToken));
                        return deleteRegistrationResult.Accept(new DeleteRegistrationResultVisitor());
                    }

                    var deleteDeviceResult = await _deviceRepositoryService.Delete(userDevice.DeviceId, accessToken);
                    await deleteDeviceResult.Accept(new DeleteDeviceAuditingVisitor(
                        _logger,
                        _auditor,
                        accessToken,
                        AuditingOperations.UsersDeviceDeleteAuditTypeResponse));
                    return deleteDeviceResult.Accept(new DeleteDeviceResultVisitor());
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

                await _auditor.AuditSecureTokenEvent(accessToken, Supplier.Microsoft,
                    AuditingOperations.UsersDevicePostAuditTypeRequest,
                    "Attempting to register user notification registration");

                try
                {
                    var registrationResponse = await _notificationService.Register(model, accessToken);

                    if (!(registrationResponse is RegistrationResult.Success successRegistrationResult))
                    {
                        await registrationResponse.Accept(
                            new NotificationRegistrationAuditingVisitor(_auditor, _logger, accessToken));
                        return registrationResponse.Accept(new NotificationRegistrationResultVisitor());
                    }

                    var deviceRepositoryResult =
                        await _deviceRepositoryService.Create(successRegistrationResult.Response, model, accessToken);
                    await deviceRepositoryResult.Accept(
                        new RegistrationRepositoryAuditingVisitor(_auditor, _logger, accessToken));
                    return deviceRepositoryResult.Accept(new DeviceRegistrationResultVisitor(model));
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

        private async Task<IActionResult> VerifyRegistration
        (
            SearchDeviceResult.Found foundDeviceResult,
            AccessToken accessToken
        )
        {
            var registrationResult = await _notificationService.Exists(foundDeviceResult.UserDevice);

            switch (registrationResult)
            {
                case RegistrationExistsResult.Found _:
                    await registrationResult.Accept(
                        new RegistrationExistsAuditingVisitor(_logger, _auditor, accessToken));
                    return foundDeviceResult.Accept(new SearchDeviceResultVisitor());

                case RegistrationExistsResult.NotFound _:
                    return await DeleteOrphanDeviceRecord(foundDeviceResult.UserDevice, accessToken);

                default:
                    await registrationResult.Accept(
                        new RegistrationExistsAuditingVisitor(_logger, _auditor, accessToken));
                    return registrationResult.Accept(new RegistrationExistsResultVisitor());
            }
        }

        private async Task<IActionResult> DeleteOrphanDeviceRecord(
            UserDevice userDevice,
            AccessToken accessToken)
        {
            var deleteDeviceResult = await _deviceRepositoryService.Delete(userDevice.DeviceId, accessToken);

            if (deleteDeviceResult is DeleteDeviceResult.Success)
            {
                var result = new SearchDeviceResult.NotFound();
                await result.Accept(
                    new SearchDeviceAuditingVisitor(_logger,
                        _auditor,
                        accessToken,
                        AuditingOperations.UsersDeviceGetAuditTypeResponse));
                return result.Accept(new SearchDeviceResultVisitor());
            }

            await deleteDeviceResult.Accept(
                new DeleteDeviceAuditingVisitor(_logger,
                    _auditor,
                    accessToken,
                    AuditingOperations.UsersDeviceGetAuditTypeResponse));
            return deleteDeviceResult.Accept(new DeleteDeviceResultVisitor());
        }
    }
}