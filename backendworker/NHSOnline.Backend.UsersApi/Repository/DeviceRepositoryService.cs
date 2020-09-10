using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UsersApi.Areas.Devices;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.Repository
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

        public async Task<RegisterDeviceResult> Create
            (NotificationRegistrationResult registration, RegisterDeviceRequest request, AccessToken accessToken)
        {
            try
            {
                _logger.LogEnter();
                var userDevice = new UserDevice
                {
                    DeviceId = _deviceIdGenerator.Generate(accessToken, request),
                    NhsLoginId = accessToken.Subject,
                    PnsToken = request.DevicePns,
                    RegistrationId = registration.Id
                };

                var result = await _deviceRepository.Create(userDevice);
                return result.Accept(new RepositoryCreateResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "User Device Registration failed with exception");
                return new RegisterDeviceResult.InternalServerError();
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
                var repositoryResult = await _deviceRepository.Find(accessToken.Subject, deviceId);
                return repositoryResult.Accept(new RepositoryGetResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "User Device find failed with exception");
                return new SearchDeviceResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<SearchDeviceResult> FindRegistrations(int maxRecords)
        {
            _logger.LogEnter();

            try
            {
                var repositoryResult = await _deviceRepository.FindRegistrations(maxRecords);
                return repositoryResult.Accept(new RepositoryFindResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "User Device find failed with exception");
                return new SearchDeviceResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<DeleteDeviceResult> Delete(string deviceId, string nhsLoginId)
        {
            _logger.LogEnter();

            try
            {
                var result = await _deviceRepository.Delete(nhsLoginId, deviceId);
                return result.Accept(new RepositoryDeleteResultVisitor(deviceId));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "User Device deletion failed with exception");
                return new DeleteDeviceResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<UpdateDeviceResult> Update(string deviceId, string nhsLoginId, string registrationId)
        {
            _logger.LogEnter();

            try
            {
                var recordBuilder = new UpdateRecordBuilder<UserDevice>();
                recordBuilder.Set(x => x.RegistrationId, registrationId);
                recordBuilder.Build();

                var result = await _deviceRepository.UpdateOne(nhsLoginId, deviceId, recordBuilder);
                return result.Accept(new RepositoryUpdateResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "User Device update failed with exception");
                return new UpdateDeviceResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
