using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public class MessagePatchResultVisitor : IMessagePatchResultVisitor<IActionResult>
    {
        public IActionResult Visit(MessagePatchResult.Updated result){
            return new NoContentResult();
        }
        
        public IActionResult Visit(MessagePatchResult.BadRequest result){
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
        
        public IActionResult Visit(MessagePatchResult.NotFound result)
        {
            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }
        
        public IActionResult Visit(MessagePatchResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        
        public IActionResult Visit(MessagePatchResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
    }
}