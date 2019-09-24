using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    internal class DeviceRepositoryService : IDeviceRepositoryService
    {
        private readonly IUserDeviceRepository _deviceRepository;
        private readonly IDeviceIdGenerator _deviceIdGenerator;
        private readonly ILogger<DevicesController> _logger;

        public DeviceRepositoryService
        (
            IUserDeviceRepository deviceRepository,
            IDeviceIdGenerator deviceIdGenerator,
            ILogger<DevicesController> logger
        )
        {
            _deviceRepository = deviceRepository;
            _deviceIdGenerator = deviceIdGenerator;
            _logger = logger;
        }

        public async Task<DeviceRegistrationResult> Create
        (
            NotificationRegistrationResult registration,
            RegisterDeviceRequest request,
            AccessToken accessToken
        )
        {
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
                return new DeviceRegistrationResult.Created(userDevice);
            }
            catch (MongoException e)
            {
                _logger.LogError($"User Device Registration failed with exception: {e}");
                return new DeviceRegistrationResult.BadGateway();
            }
            catch (Exception e)
            {
                _logger.LogError($"User Device Registration failed with exception: {e}");
                return new DeviceRegistrationResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<SearchDeviceResult> Find(string devicePns, AccessToken accessToken)
        {
            _logger.LogEnter();

            try
            {
                var deviceId = _deviceIdGenerator.Generate(accessToken, devicePns);
                var userDevice = await _deviceRepository.Find(accessToken.Subject, deviceId);

                if (userDevice != null)
                {
                    return new SearchDeviceResult.Found(userDevice);
                }

                return new SearchDeviceResult.NotFound();
            }
            catch (MongoException e)
            {
                _logger.LogError($"User Device find failed with exception: {e}");
                return new SearchDeviceResult.BadGateway();
            }
            catch (Exception e)
            {
                _logger.LogError($"User Device find failed with exception: {e}");
                return new SearchDeviceResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<DeleteDeviceResult> Delete(string deviceId, AccessToken accessToken)
        {
            _logger.LogEnter();

            try
            {
                await _deviceRepository.Delete(accessToken.Subject, deviceId);
                return new DeleteDeviceResult.Success(deviceId);
            }
            catch (MongoException e)
            {
                _logger.LogError($"User Device deletion failed with exception: {e}");
                return new DeleteDeviceResult.BadGateway();
            }
            catch (Exception e)
            {
                _logger.LogError($"User Device deletion failed with exception: {e}");
                return new DeleteDeviceResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}