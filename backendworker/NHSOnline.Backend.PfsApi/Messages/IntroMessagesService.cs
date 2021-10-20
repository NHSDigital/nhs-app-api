using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.MessagesApi.Areas.Messages;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Metrics.EventHub;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Messages
{
    public class IntroMessagesService : IIntroMessagesService
    {
        private const string IntroductoryMessageSender = "NHS App";
        private const string IntroductoryMessageSupplierId = "278d3b75-3498-4d68-8991-506d0006e46f";
        private const int IntroductoryMessageVersion = 1;

        private readonly ILogger<IntroMessagesService> _logger;
        private readonly IIntroMessagesServiceConfig _introMessagesServiceConfig;
        private readonly IMessageService _messageService;
        private readonly IEventHubLogger _eventHubLogger;

        private readonly IMapper<SenderContext, SenderContextEventLogData>
            _senderContextEventLogDataMapper;

        public IntroMessagesService(
            ILogger<IntroMessagesService> logger,
            IIntroMessagesServiceConfig introMessagesServiceConfig,
            IMessageService messageService,
            IEventHubLogger eventHubLogger,
            IMapper<SenderContext, SenderContextEventLogData> senderContextEventLogDataMapper
        )
        {
            _logger = logger;
            _introMessagesServiceConfig = introMessagesServiceConfig;
            _messageService = messageService;
            _eventHubLogger = eventHubLogger;
            _senderContextEventLogDataMapper = senderContextEventLogDataMapper;
        }

        public async Task<MessagesResult> SendIntroductoryMessage(string nhsLoginId)
        {
            _logger.LogEnter();

            try
            {
                if (!_introMessagesServiceConfig.SendIntroductoryMessage)
                {
                    return new MessagesResult.NoActionTaken();
                }

                var addMessageRequest = BuildRequest(nhsLoginId);
                var addMessageResult =
                    await _messageService.Send(addMessageRequest, nhsLoginId);

                await addMessageResult.Accept(
                    new AddMessageLogResultVisitor(_eventHubLogger, _senderContextEventLogDataMapper, _logger));

                if (addMessageResult is AddMessageResult.Success success)
                {
                    return new MessagesResult.Success(success.UserMessage.Id.ToString());
                }
                else
                {
                    _logger.LogWarning($"Failed to send introductory message for nhs login id {nhsLoginId}");
                    return new MessagesResult.InternalServerError();
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private AddMessageRequest BuildRequest(string nhsLoginId) => new AddMessageRequest
        {
            Body = _introMessagesServiceConfig.Body,
            Sender = IntroductoryMessageSender,
            Version = IntroductoryMessageVersion,
            SenderContext = new AddMessageSenderContext
            {
                CampaignId = _introMessagesServiceConfig.CampaignId,
                NhsLoginId = nhsLoginId,
                SupplierId = IntroductoryMessageSupplierId
            }
        };
    }
}
