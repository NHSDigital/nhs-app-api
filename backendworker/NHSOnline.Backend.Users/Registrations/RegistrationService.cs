using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Users.Areas.Devices.Models;
using NHSOnline.Backend.Users.Notifications;
using NHSOnline.Backend.Users.Notifications.Models;
using NHSOnline.Backend.Users.Repository;

namespace NHSOnline.Backend.Users.Registrations
{
    public class RegistrationService : IRegistrationService
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

                var installationRequest = new InstallationRequest
                {
                    DevicePns = request.DevicePns,
                    DeviceType = request.DeviceType.Value,
                    NhsLoginId = accessToken.Subject,
                    InstallationId = request.InstallationId
                };

                var registrationResult = await _notificationService.Register(installationRequest);

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
                return searchDeviceResult.Accept(new GetRegistrationServiceResultVisitor());
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
                var deleteRegistrationResult = await _notificationService.Delete(
                    userDevice.RegistrationId,
                    accessToken.Subject
                );

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
