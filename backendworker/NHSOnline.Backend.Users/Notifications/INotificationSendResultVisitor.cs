namespace NHSOnline.Backend.Users.Notifications
{
    public interface INotificationSendResultVisitor<out T>
    {
        T Visit(NotificationSendResult.BadGateway result);
        T Visit(NotificationSendResult.Conflict result);
        T Visit(NotificationSendResult.InternalServerError result);
        T Visit(NotificationSendResult.Success result);
    }
}