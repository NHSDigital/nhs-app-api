using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;
using Options = NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages.Mappers
{
    public class MessageLinkClickedDataMapper : IMapper<MessageLink, RepositoryFindResult<UserMessage>.Found, MessageLinkClickedData>
    {
        private readonly ILogger<MessageLinkClickedDataMapper> _logger;

        public MessageLinkClickedDataMapper(ILogger<MessageLinkClickedDataMapper> logger)
        {
            _logger = logger;
        }

        public MessageLinkClickedData Map(MessageLink firstSource, RepositoryFindResult<UserMessage>.Found secondSource)
        {
            ValidateMessageLink(firstSource);

            var userMessage = secondSource?.Records?.FirstOrDefault();

            return new MessageLinkClickedData(
                firstSource.MessageId,
                firstSource.Link,
                userMessage?.SenderContext?.CampaignId,
                userMessage?.SenderContext?.CommunicationId ?? userMessage?.CommunicationId,
                userMessage?.SenderContext?.TransmissionId ?? userMessage?.TransmissionId
            );
        }

        private void ValidateMessageLink(MessageLink input)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(input, nameof(input), Options.ThrowError)
                .IsNotNullOrWhitespace(input.MessageId, nameof(input.MessageId), Options.ThrowError)
                .IsNotNull(input.Link, nameof(input.Link), Options.ThrowError)
                .IsValid();
        }
    }
}
