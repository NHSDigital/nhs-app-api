using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.AspNet.ApiKey;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    [Route("api/users/devices/migration")]
    [Authorize(AuthenticationSchemes = ApiKeyAuthenticationOptions.DefaultScheme)]
    public class MigrationController : Controller
    {
        private readonly IMigrationService _migrationService;
        private readonly INotificationRegistrationService _notificationRegistrationService;
        private readonly IDeviceRepositoryService _deviceRepositoryService;
        private readonly ILogger<MigrationController> _logger;
        private readonly Regex _deviceTokenRegex = new Regex("^[a-fA-F0-9]+$");

        public MigrationController
        (
            ILogger<MigrationController> logger,
            IMigrationService migrationService,
            INotificationRegistrationService notificationRegistrationService,
            IDeviceRepositoryService deviceRepositoryService
        )
        {
            _logger = logger;
            _migrationService = migrationService;
            _notificationRegistrationService = notificationRegistrationService;
            _deviceRepositoryService = deviceRepositoryService;
        }

        [HttpPost]
        public async Task<IActionResult> MigrateRegistration([FromQuery] int migratorCount)
        {
            try
            {
                _logger.LogEnter();
                if (migratorCount <= 0)
                {
                    return BadRequest();
                }

                var migrationResult = new MigrationResult();
                var userDevicesResult = await _deviceRepositoryService.FindRegistrations(migratorCount);

                SearchDeviceResult.FoundMany userDevicesFoundManyResult;
                switch (userDevicesResult)
                {
                    case SearchDeviceResult.FoundMany foundMany:
                        userDevicesFoundManyResult = foundMany;
                        break;
                    case SearchDeviceResult.NotFound _:
                        return Ok(migrationResult);
                    default:
                        return userDevicesResult.Accept(new MigrationSearchDeviceResultVisitor());
                }

                migrationResult.TotalCount = userDevicesFoundManyResult.UserDevices.Count();

                foreach (var userDevice in userDevicesFoundManyResult.UserDevices)
                {
                    var deviceType = GetDeviceType(userDevice.PnsToken);

                    if (await VerifyDeviceRegistrationExists(userDevice, migrationResult))
                    {
                        var registrationResult =
                            await _migrationService.Register(userDevice.PnsToken, deviceType, userDevice.NhsLoginId);

                        if (registrationResult is RegistrationResult.Success registrationSuccessResult)
                        {
                            var deleteRegistrationId = await UpdateUserDeviceRecord(registrationSuccessResult,
                                userDevice, migrationResult);

                            await DeleteRegistration(deleteRegistrationId, migrationResult);
                        }
                        else
                        {
                            migrationResult.FailedCount++;
                        }
                    }
                }

                return Ok(migrationResult);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RegistrationDelete(string registrationId)
        {
            try
            {
                _logger.LogEnter();
                var result = await _migrationService.Delete(registrationId);

                return result.Accept(new DeleteRegistrationResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task DeleteRegistration(string deleteRegistrationId, MigrationResult migrationResult)
        {
            var deleteRegistrationResult = await _migrationService.Delete(deleteRegistrationId);
            switch (deleteRegistrationResult)
            {
                case DeleteRegistrationResult.Success _:
                case DeleteRegistrationResult.NotFound _:
                    break;
                default:
                    _logger.LogWarning(
                        $"MIGRATION: Failed to delete {deleteRegistrationId} notification registration");
                    migrationResult.FailedDeleteCount++;
                    break;
            }
        }

        private async Task<bool> VerifyDeviceRegistrationExists(UserDevice userDevice, MigrationResult migrationResult)
        {
            switch (await _notificationRegistrationService.Exists(userDevice))
            {
                case RegistrationExistsResult.NotFound _:
                    switch (await _deviceRepositoryService.Delete(userDevice.DeviceId, userDevice.NhsLoginId))
                    {
                        case DeleteDeviceResult.Success _:
                            migrationResult.DeletedOrphanCount++;
                            break;
                        default:
                            migrationResult.FailedDeleteOrphanCount++;
                            break;
                    }

                    return false;
                case RegistrationExistsResult.Found _:
                    return true;
                default:
                    migrationResult.FailedCount++;
                    return false;
            }
        }

        private async Task<string> UpdateUserDeviceRecord
        (
            RegistrationResult.Success registrationSuccessResult,
            UserDevice userDevice,
            MigrationResult migrationResult
        )
        {
            var registrationId = registrationSuccessResult.Response.Id;

            var updateDeviceResult = await _deviceRepositoryService.Update(userDevice.DeviceId,
                userDevice.NhsLoginId, registrationId);

            switch (updateDeviceResult)
            {
                case UpdateDeviceResult.Updated _:
                    migrationResult.SuccessCount++;
                    return userDevice.RegistrationId;
                default:
                    migrationResult.FailedCount++;
                    return registrationId;
            }
        }

        private DeviceType GetDeviceType(string pnsToken)
            => _deviceTokenRegex.IsMatch(pnsToken) ? DeviceType.Ios : DeviceType.Android;

        internal class MigrationResult
        {
            public int TotalCount { get; set; }
            public int SuccessCount { get; set; }
            public int FailedCount { get; set; }
            public int FailedDeleteCount { get; set; }
            public int DeletedOrphanCount { get; set; }
            public int FailedDeleteOrphanCount { get; set; }
        }
    }
}
