using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public class MessageResultVisitor : IMessagesResultVisitor<IActionResult>
    {
        public IActionResult Visit(MessagesResult.Some result)
        {
            var message = result?.Response?.FirstOrDefault()?.Messages?.FirstOrDefault();

            if (message == null)
            {
                return new NotFoundResult();
            }

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
