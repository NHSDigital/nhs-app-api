using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Metrics.EventHub;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public class AddMessageLogResultVisitor : IAddMessageResultVisitor<Task>
    {
        private readonly IEventHubLogger _eventHubLogger;
        private readonly IMapper<SenderContext, SenderContextEventLogData> _mapper;
        private readonly ILogger _logger;

        public AddMessageLogResultVisitor(
            IEventHubLogger eventHubLogger,
            IMapper<SenderContext, SenderContextEventLogData> mapper,
            ILogger logger
        )
        {
            _eventHubLogger = eventHubLogger;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Visit(AddMessageResult.Success result)
        {
            try
            {
                if (result.UserMessage.SenderContext == null)
                {
                    return;
                }

                var senderContextLogData = _mapper.Map(result.UserMessage.SenderContext);

                await _eventHubLogger.MessageCreated(new MessageCreatedEventLogData(
                    result.UserMessage.Id.ToString(),
                    senderContextLogData
                ));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when logging create result");
            }
        }

        public Task Visit(AddMessageResult.BadGateway result) => Task.CompletedTask;

        public Task Visit(AddMessageResult.BadRequest result) => Task.CompletedTask;

        public Task Visit(AddMessageResult.InternalServerError result) => Task.CompletedTask;
    }
}