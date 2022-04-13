using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public class AddMessageResultVisitor : IAddMessageResultVisitor<IActionResult>
    {
        public IActionResult Visit(AddMessageResult.Success result)
        {
            return new CreatedResult(string.Empty, new AddMessageResponse
            {
                MessageId = result.UserMessage.Id.ToString()
            });
        }

        public IActionResult Visit(AddMessageResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(AddMessageResult.BadRequest result)
        {
            return new BadRequestResult();
        }

        public IActionResult Visit(AddMessageResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}