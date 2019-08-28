using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public class DeviceRepositoryService : IDeviceRepositoryService
    {
        private readonly IUserDeviceRepository _deviceRepository;
        private readonly IDeviceIdGenerator _deviceIdGenerator;
        private readonly ILogger<DevicesController> _logger;

        public DeviceRepositoryService(
            IUserDeviceRepository deviceRepository,
            IDeviceIdGenerator deviceIdGenerator,
            ILogger<DevicesController> logger)
        {
            _deviceRepository = deviceRepository;
            _deviceIdGenerator = deviceIdGenerator;
            _logger = logger;
        }

        public async Task<DeviceRepositoryResult> Create(
                NotificationRegistrationResult registration, 
                RegisterDeviceRequest request,
                AccessToken accessToken) {
            _logger.LogEnter();
            var userDevice = new UserDevice
            {
                DeviceId = _deviceIdGenerator.Generate(accessToken, request),
                NhsLoginId = accessToken.Subject,
                PnsToken = request.DevicePns,
                RegistrationId = registration.RegistrationId,
                RegistrationExpiry = registration.RegistrationExpiry
            };

            try
            {
                await _deviceRepository.Create(userDevice);
                return new DeviceRepositoryResult.Created(userDevice);
            }
            catch (Exception e)
            {
                _logger.LogError($"User Device Registration failed with exception: {e}");
                return new DeviceRepositoryResult.Failure();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}