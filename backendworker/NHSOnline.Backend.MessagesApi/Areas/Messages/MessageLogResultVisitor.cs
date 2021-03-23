using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Metrics.EventHub;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public class MessageLogResultVisitor : IMessageResultVisitor<Task>
    {
        private readonly IEventHubLogger _eventHubLogger;

        private readonly IMapper<SenderContext, MessageSenderContextEventLogData>
            _messageSenderContextEventLogDataMapper;

        private readonly ILogger _logger;

        public MessageLogResultVisitor(
            IEventHubLogger eventHubLogger,
            IMapper<SenderContext, MessageSenderContextEventLogData> messageSenderContextEventLogDataMapper,
            ILogger logger)
        {
            _eventHubLogger = eventHubLogger;
            _messageSenderContextEventLogDataMapper = messageSenderContextEventLogDataMapper;
            _logger = logger;
        }

        public async Task Visit(MessageResult.Success result)
        {
            try
            {

                if (result.UserMessage.SenderContext == null)
                {
                    return;
                }

                var messageSenderContextEventLogData =
                    _messageSenderContextEventLogDataMapper.Map(result.UserMessage.SenderContext);

                await _eventHubLogger.MessageCreated(new MessageCreatedEventLogData(messageSenderContextEventLogData));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when logging create result");
            }
        }

        public Task Visit(MessageResult.BadGateway result)
            => Task.CompletedTask;

        public Task Visit(MessageResult.BadRequest result)
            => Task.CompletedTask;

        public Task Visit(MessageResult.InternalServerError result)
            => Task.CompletedTask;
    }
}