using NHSOnline.Backend.Messages.Areas.Messages;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Users.Notifications
{
    public class NotificationBadgeCountResultVisitor : IUnreadMessageCountResultVisitor<long>
    {
        public long Visit(UnreadMessageCountResult.Success result) => result.UnreadMessageCount;

        public long Visit(UnreadMessageCountResult.Failure result) => 0;
    }
}