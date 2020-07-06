using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public class MessageResultVisitor : IMessageResultVisitor<IActionResult>
    {
        public IActionResult Visit(MessageResult.Success result){
            return new CreatedResult(string.Empty, result.Response);
        }

        public IActionResult Visit(MessageResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(MessageResult.BadRequest result)
        {
            return new BadRequestResult();
        }

        public IActionResult Visit(MessageResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}