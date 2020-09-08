namespace NHSOnline.Backend.UsersApi.Notifications
{
    public abstract class NotificationSendResult
    {
        public abstract T Accept<T>(INotificationSendResultVisitor<T> visitor);

        public class BadGateway : NotificationSendResult
        {
            public override T Accept<T>(INotificationSendResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : NotificationSendResult
        {
            public override T Accept<T>(INotificationSendResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Success : NotificationSendResult
        {
            public override T Accept<T>(INotificationSendResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}