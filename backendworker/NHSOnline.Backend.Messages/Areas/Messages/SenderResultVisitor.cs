using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public class SenderResultVisitor : ISenderResultVisitor<IActionResult>
    {
        public IActionResult Visit(SenderResult.Found result)
        {
            var sender = result?.Response;
            return new OkObjectResult(sender);
        }

        public IActionResult Visit(SenderResult.NotFound result)
        {
            return new NotFoundResult();
        }

        public IActionResult Visit(SenderResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(SenderResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}