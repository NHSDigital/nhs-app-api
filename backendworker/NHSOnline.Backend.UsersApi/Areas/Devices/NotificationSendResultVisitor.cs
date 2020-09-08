using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public class NotificationSendResultVisitor: INotificationSendResultVisitor<IActionResult>
    {
        public IActionResult Visit(NotificationSendResult.Success result)
        {
           return new AcceptedResult();
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