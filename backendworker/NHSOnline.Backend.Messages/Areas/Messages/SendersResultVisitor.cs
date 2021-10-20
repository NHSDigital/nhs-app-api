using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public class SendersResultVisitor : ISendersResultVisitor<IActionResult>
    {
        public IActionResult Visit(SendersResult.Found result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(SendersResult.None result)
        {
            return new NoContentResult();
        }

        public IActionResult Visit(SendersResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(SendersResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}