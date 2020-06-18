using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public class RegistrationExistsResultVisitor : IRegistrationExistsResultVisitor<IActionResult>
    {
        public IActionResult Visit(RegistrationExistsResult.Found result)
            => new StatusCodeResult(StatusCodes.Status204NoContent);
        
        public IActionResult Visit(RegistrationExistsResult.NotFound result)
            => new StatusCodeResult(StatusCodes.Status404NotFound);

        public IActionResult Visit(RegistrationExistsResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(RegistrationExistsResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}