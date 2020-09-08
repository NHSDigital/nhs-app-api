using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public class FindRegistrationsResultVisitor : IFindRegistrationsResultVisitor<IActionResult>
    {
        public IActionResult Visit(FindRegistrationsResult.Found result)
        {
            return new OkObjectResult(result.RegistrationIds);
        }

        public IActionResult Visit(FindRegistrationsResult.NotFound result)
        {
            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }

        public IActionResult Visit(FindRegistrationsResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        public IActionResult Visit(FindRegistrationsResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
    }
}