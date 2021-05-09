using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.AspNet.ApiKey;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications.Migration;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public class MigrationController : Controller
    {
        private readonly ILogger<MigrationController> _logger;
        private readonly IMigrationService _migrationService;

        public MigrationController(ILogger<MigrationController> logger, IMigrationService migrationService)
        {
            _logger = logger;
            _migrationService = migrationService;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = ApiKeyAuthenticationOptions.DefaultScheme)]
        [Route("api/users/migration")]
        public async Task<IActionResult> Migrate([FromBody] MigrationRequest request)
        {
            try
            {
                _logger.LogEnter();
                _logger.LogInformation($"Migrating {request}");

                var result = await _migrationService.Migrate(request);

                if (result is MigrationResult.Success success)
                {
                    _logger.LogInformation($"Successfully migrated {request}, new installation id: {success.InstallationId}");
                }

                return result.Accept(new MigrationResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Migration failed with exception");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
