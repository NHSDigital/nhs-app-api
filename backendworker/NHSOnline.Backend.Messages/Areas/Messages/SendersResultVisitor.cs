using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public class SendersResultVisitor : ISendersResultVisitor<IActionResult>
    {
        public IActionResult Visit(SendersResult.Found result)
        {
            var senderIds = result?.Response.Senders.Select(x => x.Id).ToList();
            return new OkObjectResult(senderIds);
        }

        public IActionResult Visit(SendersResult.None result)
        {
            return new OkObjectResult(new List<string>());
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