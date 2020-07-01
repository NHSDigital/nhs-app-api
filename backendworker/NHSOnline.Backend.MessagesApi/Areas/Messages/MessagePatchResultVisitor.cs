using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Metrics;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public class MessagePatchResultVisitor : IMessagePatchResultVisitor<Task<IActionResult>>
    {
        private readonly IMetricLogger _metricLogger;

        public MessagePatchResultVisitor(IMetricLogger metricLogger)
        {
            _metricLogger = metricLogger;
        }

        public async Task<IActionResult> Visit(MessagePatchResult.Updated result)
        {
            await _metricLogger.MessageRead();
            return new NoContentResult();
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