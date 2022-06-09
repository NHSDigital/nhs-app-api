using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public class SenderPostResultVisitor : ISenderPostResultVisitor<IActionResult>
    {
        public IActionResult Visit(SenderPostResult.Success result)
        {
            return new CreatedResult(string.Empty, new Sender()
            {
                Id = result.DbSender.Id,
                Name = result.DbSender.Name
            });
        }

        public IActionResult Visit(SenderPostResult.Created result)
        {
            return new NoContentResult();
        }

        public IActionResult Visit(SenderPostResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        public IActionResult Visit(SenderPostResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
    }
}