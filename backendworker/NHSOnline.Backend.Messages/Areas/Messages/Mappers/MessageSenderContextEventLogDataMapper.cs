using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Metrics.EventHub;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.Messages.Areas.Messages.Mappers
{
    public class MessageSenderContextEventLogDataMapper : IMapper<SenderContext, SenderContextEventLogData>
    {
        private readonly ILogger<MessageSenderContextEventLogDataMapper> _logger;

        public MessageSenderContextEventLogDataMapper(ILogger<MessageSenderContextEventLogDataMapper> logger)
        {
            _logger = logger;
        }

        public SenderContextEventLogData Map(SenderContext source)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(source, "source", ThrowError)
                .IsValid();

            return new SenderContextEventLogData(
                source.SupplierId,
                source.CommunicationId,
                source.TransmissionId,
                source.CommunicationCreatedDateTime,
                source.RequestReference,
                source.CampaignId,
                source.OdsCode,
                source.NhsNumber,
                source.NhsLoginId
            );
        }
    }
}