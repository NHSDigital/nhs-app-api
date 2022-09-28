using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Auth;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Metrics.EventHub;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public class MessageLogPatchResultVisitor : IMessagePatchResultVisitor<Task>
    {
        private readonly IMetricLogger<UserSessionMetricContext> _metricLogger;
        private readonly IEventHubLogger _eventHubLogger;
        private readonly ILogger _logger;

        private readonly IMapper<SenderContext, SenderContextEventLogData>
            _messageSenderContextEventLogDataMapper;

        public MessageLogPatchResultVisitor(
            IMetricLogger<UserSessionMetricContext> metricLogger,
            IEventHubLogger eventHubLogger,
            IMapper<SenderContext, SenderContextEventLogData> messageSenderContextEventLogDataMapper,
            ILogger logger)
        {
            _metricLogger = metricLogger;
            _eventHubLogger = eventHubLogger;
            _messageSenderContextEventLogDataMapper = messageSenderContextEventLogDataMapper;
            _logger = logger;
        }

        public Task Visit(MessagePatchResult.BadRequest result) => Task.CompletedTask;

        public Task Visit(MessagePatchResult.NoChange result) => Task.CompletedTask;

        public async Task Visit(MessagePatchResult.Updated result)
        {
            var tasks = new List<Task>();
            try
            {
                switch (result.MessagePatchType)
                {
                    case MessagePatchType.Read:
                        if (result.UserMessage.SenderContext != null)
                        {
                            var messageSenderContextEventLogData =
                                _messageSenderContextEventLogDataMapper.Map(result.UserMessage.SenderContext);

                            tasks.Add(_eventHubLogger.MessageRead(new MessageReadEventLogData(
                                result.UserMessage.Id.ToString(), messageSenderContextEventLogData)));
                        }

                        tasks.Add(_metricLogger.MessageRead(
                            new MessageReadData(
                                result.UserMessage.Id.ToString(),
                                result.UserMessage.SenderContext?.CommunicationId,
                                result.UserMessage.SenderContext?.TransmissionId,
                                result.UserMessage.SenderContext?.CampaignId,
                                result.UserMessage.SenderContext?.SupplierId)
                        ));
                        break;
                    case MessagePatchType.Reply:
                        var messageReplySenderContextEventLogData =
                            _messageSenderContextEventLogDataMapper.Map(result.UserMessage.SenderContext);
                        tasks.Add(_eventHubLogger.MessageReplySent(new MessageReplySentEventLogData(
                            result.UserMessage.Id.ToString(), result.UserMessage.Reply?.Response,
                            messageReplySenderContextEventLogData)));
                        break;
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when logging patch result");
            }
        }

        public Task Visit(MessagePatchResult.NotFound result) => Task.CompletedTask;

        public Task Visit(MessagePatchResult.BadGateway result) => Task.CompletedTask;

        public Task Visit(MessagePatchResult.InternalServerError result) => Task.CompletedTask;

    }
}