using Microsoft.Extensions.Logging;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public class MessageLinkClickedValidationService : IMessageLinkClickedValidationService
    {
        private readonly ILogger<MessageLinkClickedValidationService> _logger;

        public MessageLinkClickedValidationService(ILogger<MessageLinkClickedValidationService> logger)
        {
            _logger = logger;
        }

        public bool IsServiceRequestValid(string nhsLoginId, MessageLink messageLink)
        {
            return new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(nhsLoginId, nameof(nhsLoginId))
                .IsNotNull(messageLink, nameof(messageLink))
                .IsNotNullOrWhitespace(messageLink.MessageId, nameof(messageLink.MessageId))
                .IsNotNull(messageLink.Link, nameof(messageLink.Link))
                .IsValid();
        }
    }
}