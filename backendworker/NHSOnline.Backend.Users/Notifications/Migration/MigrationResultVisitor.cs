using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Users.Areas.Devices.Models;

namespace NHSOnline.Backend.Users.Notifications.Migration
{
    public class MigrationResultVisitor : IMigrationResultVisitor<IActionResult>
    {
        public IActionResult Visit(MigrationResult.Success result)
        {
            return new OkObjectResult(new MigrationResponse
            {
                InstallationId = result.InstallationId
            });
        }

        public IActionResult Visit(MigrationResult.BadRequest result)
        {
            return new BadRequestObjectResult(new MigrationResponse
            {
                ErrorMessage = result.ErrorMessage
            });
        }

        public IActionResult Visit(MigrationResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(MigrationResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
