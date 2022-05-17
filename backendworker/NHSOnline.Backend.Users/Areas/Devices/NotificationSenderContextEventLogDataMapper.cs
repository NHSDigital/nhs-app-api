using NHSOnline.Backend.Metrics.EventHub;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Users.Areas.Devices.Models;

namespace NHSOnline.Backend.Users.Areas.Devices
{
    public class NotificationSenderContextEventLogDataMapper : IMapper<AddNotificationSenderContext, SenderContextEventLogData>
    {
        public SenderContextEventLogData Map(AddNotificationSenderContext source)
        {
            if (source == null)
            {
                return new SenderContextEventLogData();
            }

            return new SenderContextEventLogData(
                source.SupplierId,
                source.SenderId,
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