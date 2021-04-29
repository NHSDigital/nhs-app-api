using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.Messages.Models;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Messages
{
    public class MessagesService : IMessagesService
    {
        private const string IntroductoryMessageSender = "NHS App";
        private const string IntroductoryMessageSupplierId = "278d3b75-3498-4d68-8991-506d0006e46f";
        private const int IntroductoryMessageVersion = 1;

        private readonly ILogger<MessagesService> _logger;
        private readonly IMessagesClient _messagesClient;
        private readonly IMessagesServiceConfig _messagesServiceConfig;

        public MessagesService(
            ILogger<MessagesService> logger,
            IMessagesClient messagesClient,
            IMessagesServiceConfig messagesServiceConfig
        )
        {
            _logger = logger;
            _messagesClient = messagesClient;
            _messagesServiceConfig = messagesServiceConfig;
        }

        public async Task<MessagesResult> SendIntroductoryMessage(string nhsLoginId)
        {
            _logger.LogEnter();

            try
            {
                if (!_messagesServiceConfig.SendIntroductoryMessage)
                {
                    return new MessagesResult.NoActionTaken();
                }

                var request = BuildRequest(nhsLoginId);
                var response = await _messagesClient.Post(request);

                if (response.HasSuccessResponse)
                {
                    return new MessagesResult.Success(response.MessageId);
                }
                else
                {
                    _logger.LogWarning($"Failed to send introductory message for nhs login id {nhsLoginId}");
                    return new MessagesResult.BadGateway();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Messages API failed with exception");
                return new MessagesResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private AddMessageRequest BuildRequest(string nhsLoginId) => new AddMessageRequest
        {
            Body = _messagesServiceConfig.Body,
            Sender = IntroductoryMessageSender,
            Version = IntroductoryMessageVersion,
            SenderContext = new AddMessageSenderContext
            {
                CampaignId = _messagesServiceConfig.CampaignId,
                NhsLoginId = nhsLoginId,
                SupplierId = IntroductoryMessageSupplierId
            }
        };
    }
}
