using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Metrics.EventHub;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public class MessageLogPatchResultVisitor : IMessagePatchResultVisitor<Task>
    {
        private readonly IMetricLogger _metricLogger;
        private readonly IEventHubLogger _eventHubLogger;
        private readonly ILogger _logger;

        private readonly IMapper<SenderContext, MessageSenderContextEventLogData>
            _messageSenderContextEventLogDataMapper;

        public MessageLogPatchResultVisitor(
            IMetricLogger metricLogger,
            IEventHubLogger eventHubLogger,
            IMapper<SenderContext, MessageSenderContextEventLogData> messageSenderContextEventLogDataMapper,
            ILogger logger)
        {
            _metricLogger = metricLogger;
            _eventHubLogger = eventHubLogger;
            _messageSenderContextEventLogDataMapper = messageSenderContextEventLogDataMapper;
            _logger = logger;
        }

        public Task Visit(MessagePatchResult.BadRequest result)
            => Task.CompletedTask;

        public Task Visit(MessagePatchResult.NoChange result)
            => Task.CompletedTask;

        public async Task Visit(MessagePatchResult.Updated result)
        {
            try
            {
                var tasks = new List<Task>();
                if (result.UserMessage.SenderContext != null)
                {
                    var messageSenderContextEventLogData =
                        _messageSenderContextEventLogDataMapper.Map(result.UserMessage.SenderContext);

                    tasks.Add(_eventHubLogger.MessageRead(new MessageReadEventLogData(messageSenderContextEventLogData)));
                }

                tasks.Add(_metricLogger.MessageRead(
                    new MessageReadData(
                        result.UserMessage.Id.ToString(),
                        result.UserMessage.CommunicationId,
                        result.UserMessage.TransmissionId,
                        result.UserMessage.SenderContext?.CampaignId,
                        result.UserMessage.SenderContext?.SupplierId)
                ));

                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when logging patch result");
            }
        }

        public Task Visit(MessagePatchResult.NotFound result)
            => Task.CompletedTask;

        public Task Visit(MessagePatchResult.BadGateway result)
            => Task.CompletedTask;

        public Task Visit(MessagePatchResult.InternalServerError result)
            => Task.CompletedTask;
    }
}