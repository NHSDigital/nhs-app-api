using Microsoft.Extensions.Logging;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Hasher;
using Options = NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages.Mappers
{
    public class MessageLinkClickedDataMapper : IMapper<MessageLink, UserMessage, MessageLinkClickedData>
    {
        private readonly ILogger<MessageLinkClickedDataMapper> _logger;
        private readonly IHashingService _hashingService;

        public MessageLinkClickedDataMapper(
            ILogger<MessageLinkClickedDataMapper> logger,
            IHashingService hashingService)
        {
            _logger = logger;
            _hashingService = hashingService;
        }

        public MessageLinkClickedData Map(MessageLink firstSource, UserMessage secondSource)
        {
            ValidateMessageLink(firstSource);
            ValidateUserMessage(secondSource);

            var mappedLink = MapLink(firstSource, secondSource);

            return new MessageLinkClickedData(
                firstSource.MessageId,
                mappedLink,
                secondSource.SenderContext?.CampaignId,
                secondSource.SenderContext?.CommunicationId,
                secondSource.SenderContext?.TransmissionId
            );
        }

        private string MapLink(MessageLink firstSource, UserMessage secondSource)
        {
            var absoluteUri = firstSource.Link.AbsoluteUri;

            return string.IsNullOrEmpty(secondSource.SenderContext?.CommunicationId) ? absoluteUri : _hashingService.Hash(absoluteUri);
        }

        private void ValidateMessageLink(MessageLink input)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(input, nameof(input), Options.ThrowError)
                .IsNotNullOrWhitespace(input.MessageId, nameof(input.MessageId), Options.ThrowError)
                .IsNotNull(input.Link, nameof(input.Link), Options.ThrowError)
                .IsValid();
        }

        private void ValidateUserMessage(UserMessage input)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(input, nameof(input), Options.ThrowError)
                .IsValid();
        }
    }
}
