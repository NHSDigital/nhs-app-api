using System.Threading.Tasks;

namespace NHSOnline.Backend.Metrics.EventHub
{
    public interface IEventHubLogger
    {
        Task MessageCreated(MessageCreatedEventLogData data);
        Task MessageRead(MessageReadEventLogData data);
        Task NotificationEnqueued(NotificationEnqueuedEventLogData data);
    }
}