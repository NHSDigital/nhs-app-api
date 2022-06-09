using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public class UserSendersResultVisitor : IUserSendersResultVisitor<IActionResult>
    {
        public IActionResult Visit(UserSendersResult.Found result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(UserSendersResult.None result)
        {
            return new NoContentResult();
        }

        public IActionResult Visit(UserSendersResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(UserSendersResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}