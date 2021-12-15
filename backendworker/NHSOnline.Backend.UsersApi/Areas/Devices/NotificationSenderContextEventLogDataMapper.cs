using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Metrics.EventHub;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public class NotificationSenderContextEventLogDataMapper : IMapper<AddNotificationSenderContext, SenderContextEventLogData>
    {
        private readonly ILogger<NotificationSenderContextEventLogDataMapper> _logger;

        public NotificationSenderContextEventLogDataMapper(ILogger<NotificationSenderContextEventLogDataMapper> logger)
        {
            _logger = logger;
        }

        public SenderContextEventLogData Map(AddNotificationSenderContext source)
        {
            if (source == null)
            {
                return new SenderContextEventLogData();
            }

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