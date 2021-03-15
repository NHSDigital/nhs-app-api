using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages.Mappers
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
                CommunicationId = MapCommunicationId(firstSource),
                TransmissionId = MapTransmissionId(firstSource),
                SenderContext = MapSenderContext(firstSource.SenderContext)
            };
        }

        private static string MapCommunicationId(AddMessageRequest request)
        {
            var communicationId = request.SenderContext?.CommunicationId ?? request.CommunicationId;

            return string.IsNullOrWhiteSpace(communicationId)
                ? null
                : communicationId;
        }

        private static string MapTransmissionId(AddMessageRequest request)
        {
            var transmissionId = request.SenderContext?.TransmissionId ?? request.TransmissionId;

            return string.IsNullOrWhiteSpace(transmissionId)
                ? null
                : transmissionId;
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
                SupplierId = senderContext.SupplierId
            };
        }
    }
}