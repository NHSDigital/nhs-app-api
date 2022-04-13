using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public class MessagesResultVisitor : IMessagesResultVisitor<IActionResult>
    {
        public IActionResult Visit(MessagesResult.Found result)
        {
            return new OkObjectResult(result.Response.SenderMessages);
        }

        public IActionResult Visit(MessagesResult.None result)
        {
            return new NoContentResult();
        }

        public IActionResult Visit(MessagesResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(MessagesResult.BadRequest result)
        {
           return new BadRequestResult();
        }

        public IActionResult Visit(MessagesResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}