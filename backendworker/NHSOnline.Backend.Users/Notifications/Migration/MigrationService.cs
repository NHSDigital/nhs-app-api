using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Users.Areas.Devices.Models;

namespace NHSOnline.Backend.Users.Notifications.Migration
{
    public class MigrationService : IMigrationService
    {
        private readonly ILogger<MigrationService> _logger;
        private readonly INotificationClient _notificationClient;

        public MigrationService(
            ILogger<MigrationService> logger,
            INotificationClient notificationClient
        )
        {
            _logger = logger;
            _notificationClient = notificationClient;
        }

        public async Task<MigrationResult> Migrate(MigrationRequest request)
        {
            try
            {
                var (statusCode, message) = await _notificationClient.Migrate(request);

                return statusCode switch
                {
                    HttpStatusCode.BadRequest => new MigrationResult.BadRequest(message),
                    HttpStatusCode.BadGateway => new MigrationResult.BadGateway(),
                    HttpStatusCode.OK => new MigrationResult.Success(message),
                    _ => throw new ArgumentOutOfRangeException(statusCode.ToString())
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to migrate registration");
                return new MigrationResult.InternalServerError();
            }
        }
    }
}
