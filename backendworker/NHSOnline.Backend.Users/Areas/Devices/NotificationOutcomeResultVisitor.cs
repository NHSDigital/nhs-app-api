using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Users.Notifications;

namespace NHSOnline.Backend.Users.Areas.Devices
{
    public class NotificationOutcomeResultVisitor: INotificationOutcomeResultVisitor<IActionResult>
    {
        public IActionResult Visit(NotificationOutcomeResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(NotificationOutcomeResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        public IActionResult Visit(NotificationOutcomeResult.NotFound result)
        {
            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }

        public IActionResult Visit(NotificationOutcomeResult.Success result)
        {
            return new OkObjectResult(result.NotificationOutcomeResponse);
        }
    }
}