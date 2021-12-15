using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Notifications.Models;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public class NotificationSendResultVisitor: INotificationSendResultVisitor<IActionResult>
    {
        public IActionResult Visit(NotificationSendResult.Success result)
        {
            return new AcceptedResult(string.Empty, result.NotificationResponse);
        }

        public IActionResult Visit(NotificationSendResult.Conflict result)
        {
            return new ConflictResult();
        }

        public IActionResult Visit(NotificationSendResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        public IActionResult Visit(NotificationSendResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
    }
}