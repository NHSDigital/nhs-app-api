using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public class DeleteRegistrationResultVisitor : IDeleteRegistrationResultVisitor<IActionResult>
    {
        public IActionResult Visit(DeleteRegistrationResult.Success result)
        {
            return new StatusCodeResult(StatusCodes.Status204NoContent);
        }

        public IActionResult Visit(DeleteRegistrationResult.NotFound result)
        {
            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }

        public IActionResult Visit(DeleteRegistrationResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(DeleteRegistrationResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}