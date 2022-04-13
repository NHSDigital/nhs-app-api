using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public class MessagePatchResultVisitor : IMessagePatchResultVisitor<Task<IActionResult>>
    {

        public async Task<IActionResult> Visit(MessagePatchResult.NoChange result)
        {
            return await Task.FromResult(new NoContentResult());
        }

        public async Task<IActionResult> Visit(MessagePatchResult.Updated result)
        {
            return await Task.FromResult(new NoContentResult());
        }

        public async Task<IActionResult> Visit(MessagePatchResult.BadRequest result){
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status400BadRequest));
        }

        public async Task<IActionResult> Visit(MessagePatchResult.NotFound result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status404NotFound));
        }

        public async Task<IActionResult> Visit(MessagePatchResult.InternalServerError result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status500InternalServerError));
        }

        public async Task<IActionResult> Visit(MessagePatchResult.BadGateway result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status502BadGateway));
        }
    }
}