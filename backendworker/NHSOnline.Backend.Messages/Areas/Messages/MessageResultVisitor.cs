using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public class MessageResultVisitor : IMessagesResultVisitor<IActionResult>
    {
        public IActionResult Visit(MessagesResult.Found result)
        {
            var message = result?.Response?.SenderMessages?.FirstOrDefault()?.Messages?.FirstOrDefault();

            return new OkObjectResult(message);
        }

        public IActionResult Visit(MessagesResult.None result)
        {
            return new NotFoundResult();
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
