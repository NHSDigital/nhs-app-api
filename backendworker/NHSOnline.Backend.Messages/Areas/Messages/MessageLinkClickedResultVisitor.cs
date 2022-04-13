using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public class MessageLinkClickedResultVisitor : IMessageLinkClickedResultVisitor<IActionResult>
    {
        public IActionResult Visit(MessageLinkClickedResult.Success result)
        {
            return new OkResult();
        }

        public IActionResult Visit(MessageLinkClickedResult.BadRequest result)
        {
            return new BadRequestResult();
        }

        public IActionResult Visit(MessageLinkClickedResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(MessageLinkClickedResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
