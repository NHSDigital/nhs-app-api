using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.MessagesApi.Repository;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Metrics.EventHub;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Hasher;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public class MessageLinkClickedService : IMessageLinkClickedService
    {
        private readonly ILogger<MessageLinkClickedService> _logger;
        private readonly IMapper<SenderContext, SenderContextEventLogData> _senderContextEventLogDataMapper;
        private readonly IMessageLinkClickedValidationService _validationService;
        private readonly IMessageRepository _messageRepository;
        private readonly IMetricLogger _metricLogger;
        private readonly IEventHubLogger _eventHubLogger;
        private readonly IHashingService _hashingService;

        public MessageLinkClickedService(
            ILogger<MessageLinkClickedService> logger,
            IMapper<SenderContext, SenderContextEventLogData> senderContextEventLogDataMapper,
            IMessageLinkClickedValidationService validationService,
            IMessageRepository messageRepository,
            IMetricLogger metricLogger,
            IEventHubLogger eventHubLogger,
            IHashingService hashingService)
        {
            _logger = logger;
            _senderContextEventLogDataMapper = senderContextEventLogDataMapper;
            _validationService = validationService;
            _messageRepository = messageRepository;
            _metricLogger = metricLogger;
            _eventHubLogger = eventHubLogger;
            _hashingService = hashingService;
        }

        public async Task<MessageLinkClickedResult> LogLinkClicked(string nhsLoginId, MessageLink messageLink)
        {
            try
            {
                _logger.LogEnter();

                if (!_validationService.IsServiceRequestValid(nhsLoginId, messageLink))
                {
                    return new MessageLinkClickedResult.BadRequest();
                }

                var result = await _messageRepository.FindMessage(nhsLoginId, messageLink.MessageId);

                if (!(result is RepositoryFindResult<UserMessage>.Found found))
                {
                    return result.Accept(new RepositoryFindMessageLinkClickedResultVisitor());
                }

                var userMessage = found.Records.First();

                await LogLinkClickedEvents(userMessage, messageLink);

                return new MessageLinkClickedResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $@"Failed to log metrics for {messageLink}");
                return new MessageLinkClickedResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task LogLinkClickedEvents(UserMessage userMessage, MessageLink messageLink)
        {
            var sanitizedLink = GetSanitizedLink(userMessage, messageLink);

            var tasks = new List<Task>();
            if (userMessage.SenderContext != null)
            {
                tasks.Add(GetEventHubLoggerTask(userMessage, sanitizedLink));
            }

            tasks.Add(GetMetricLoggerTask(userMessage, sanitizedLink));

            await Task.WhenAll(tasks);
        }

        private string GetSanitizedLink(UserMessage userMessage, MessageLink messageLink)
        {
            var link = messageLink.Link;

            return string.IsNullOrEmpty(userMessage.SenderContext?.CommunicationId) ?
                link : _hashingService.Hash(link);
        }

        private Task GetEventHubLoggerTask(UserMessage userMessage, string sanitizedLink)
        {
            var senderContextEventLogData =
                _senderContextEventLogDataMapper.Map(userMessage.SenderContext);

            var linkClickedEventLogData = new MessageLinkClickedEventLogData(
                userMessage.Id.ToString(),
                sanitizedLink,
                senderContextEventLogData);

            return _eventHubLogger.MessageLinkClicked(linkClickedEventLogData);
        }

        private Task GetMetricLoggerTask(UserMessage userMessage, string sanitizedLink)
        {
            var messageLinkClickedData = new MessageLinkClickedData(
                userMessage.Id.ToString(),
                sanitizedLink,
                userMessage.SenderContext?.CampaignId,
                userMessage.SenderContext?.CommunicationId,
                userMessage.SenderContext?.TransmissionId
            );

            return _metricLogger.MessageLinkClicked(messageLinkClickedData);
        }
    }
}
