using NHSOnline.Backend.Users.Areas.Devices.Models;

namespace NHSOnline.Backend.Users.Notifications
{
    public abstract class NotificationOutcomeResult
    {
        public abstract T Accept<T>(INotificationOutcomeResultVisitor<T> visitor);

        public class BadGateway : NotificationOutcomeResult
        {
            public override T Accept<T>(INotificationOutcomeResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : NotificationOutcomeResult
        {
            public override T Accept<T>(INotificationOutcomeResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotFound : NotificationOutcomeResult
        {
            public override T Accept<T>(INotificationOutcomeResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Success : NotificationOutcomeResult
        {
            public Success(NotificationOutcomeResponse notificationOutcomeResponse)
            {
                NotificationOutcomeResponse = notificationOutcomeResponse;
            }

            public NotificationOutcomeResponse NotificationOutcomeResponse { get; }

            public override T Accept<T>(INotificationOutcomeResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}