using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    internal class RegistrationResultVisitor : IRegistrationResultVisitor<IActionResult>
    {
        public IActionResult Visit(RegistrationResult.Success result)
        {
            return new StatusCodeResult(StatusCodes.Status201Created);
        }

        public IActionResult Visit(RegistrationResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(RegistrationResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}