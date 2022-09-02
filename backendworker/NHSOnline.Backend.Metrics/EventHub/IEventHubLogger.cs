using System.Threading.Tasks;

namespace NHSOnline.Backend.Metrics.EventHub
{
    public interface IEventHubLogger
    {
        Task MessageCreated(MessageCreatedEventLogData data);
        Task MessageLinkClicked(MessageLinkClickedEventLogData data);
        Task MessageRead(MessageReadEventLogData data);
        Task MessageReply(MessageReplyEventLogData data);
        Task NotificationEnqueued(NotificationEnqueuedEventLogData data);
    }
}