using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Registrations
{
    internal class RegistrationService : IRegistrationService
    {
        private readonly INotificationRegistrationService _notificationService;
        private readonly IDeviceRepositoryService _deviceRepositoryService;
        private readonly ILogger<RegistrationService> _logger;

        public RegistrationService
        (
            IDeviceRepositoryService deviceRepositoryService,
            INotificationRegistrationService notificationService,
            ILogger<RegistrationService> logger
        )
        {
            _notificationService = notificationService;
            _deviceRepositoryService = deviceRepositoryService;
            _logger = logger;
        }

        public async Task<RegisterDeviceResult> CreateRegistration
            (RegisterDeviceRequest request, AccessToken accessToken)
        {
            try
            {
                _logger.LogEnter();

                var registrationResult = await _notificationService.Register(request.DevicePns,
                    request.DeviceType.Value, accessToken.Subject);

                if (!(registrationResult is RegistrationResult.Success registrationSuccessResult))
                {
                    return registrationResult.Accept(new CreateRegistrationResultVisitor());
                }

                return await _deviceRepositoryService.Create(registrationSuccessResult.Response, request, accessToken);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<RegistrationExistsResult> GetRegistration(string devicePns, AccessToken accessToken)
        {
            try
            {
                _logger.LogEnter();

                var searchDeviceResult = await _deviceRepositoryService.Find(devicePns, accessToken);
                if (!(searchDeviceResult is SearchDeviceResult.Found foundDeviceResult))
                {
                    return searchDeviceResult.Accept(new GetRegistrationServiceResultVisitor());
                }

                var registrationResult = await _notificationService.Exists(foundDeviceResult.UserDevice);
                if (registrationResult is RegistrationExistsResult.NotFound)
                {
                    var deleteDeviceResult =
                        await _deviceRepositoryService.Delete(foundDeviceResult.UserDevice.DeviceId,
                            accessToken.Subject);

                    if (!(deleteDeviceResult is DeleteDeviceResult.Success))
                    {
                        return deleteDeviceResult.Accept(new GetRegistrationServiceResultVisitor());
                    }
                }

                return registrationResult;
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<FindRegistrationsResult> Find(string nhsLoginId)
        {
            try
            {
                _logger.LogEnter();

                return await _notificationService.Find(nhsLoginId);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<DeleteRegistrationResult> DeleteRegistration(string devicePns, AccessToken accessToken)
        {
            try
            {
                _logger.LogEnter();

                var searchDeviceResult = await _deviceRepositoryService.Find(devicePns, accessToken);
                if (!(searchDeviceResult is SearchDeviceResult.Found foundDeviceResult))
                {
                    return searchDeviceResult.Accept(new DeleteRegistrationServiceResultVisitor());
                }

                var userDevice = foundDeviceResult.UserDevice;
                var deleteRegistrationResult = await _notificationService.Delete(userDevice.RegistrationId);

                switch (deleteRegistrationResult)
                {
                    case DeleteRegistrationResult.Success _:
                    case DeleteRegistrationResult.NotFound _:
                        var deleteDeviceResult =
                            await _deviceRepositoryService.Delete(userDevice.DeviceId, accessToken.Subject);
                        return deleteDeviceResult.Accept(new DeleteRegistrationServiceResultVisitor());
                    default:
                        return deleteRegistrationResult;
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
