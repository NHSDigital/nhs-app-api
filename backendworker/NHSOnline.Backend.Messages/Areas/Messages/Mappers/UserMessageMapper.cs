using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.Messages.Areas.Messages.Mappers
{
    public class UserMessageMapper : IMapper<AddMessageRequest, string, UserMessage>
    {
        private readonly ILogger<UserMessageMapper> _logger;

        public UserMessageMapper(ILogger<UserMessageMapper> logger)
        {
            _logger = logger;
        }
        public UserMessage Map(AddMessageRequest firstSource, string secondSource)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(firstSource, nameof(firstSource), ThrowError)
                .IsNotNull(secondSource, nameof(secondSource), ThrowError)
                .IsValid();

            return new UserMessage
            {
                NhsLoginId = secondSource,
                Sender = firstSource.Sender,
                Version = firstSource.Version,
                Body = firstSource.Body,
                SentTime = DateTime.UtcNow,
                SenderContext = MapSenderContext(firstSource.SenderContext)
            };
        }

        private static SenderContext MapSenderContext(AddMessageSenderContext senderContext)
        {
            if (senderContext == null)
            {
                return null;
            }

            return new SenderContext
            {
                CampaignId = senderContext.CampaignId,
                CommunicationCreatedDateTime = senderContext.CommunicationCreatedDateTime,
                CommunicationId = senderContext.CommunicationId,
                TransmissionId = senderContext.TransmissionId,
                NhsLoginId = senderContext.NhsLoginId,
                NhsNumber = senderContext.NhsNumber,
                OdsCode = senderContext.OdsCode,
                RequestReference = senderContext.RequestReference,
                SenderId = senderContext.SenderId,
                SupplierId = senderContext.SupplierId
            };
        }
    }
}