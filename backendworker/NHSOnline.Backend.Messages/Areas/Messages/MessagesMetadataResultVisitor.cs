using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public class MessagesMetadataResultVisitor : IMessagesMetadataResultVisitor<IActionResult>
    {
        public IActionResult Visit(MessagesMetadataResult.Found result)
        {
            return new OkObjectResult(result.Response.MessagesMetadata);
        }

        public IActionResult Visit(MessagesMetadataResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(MessagesMetadataResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}