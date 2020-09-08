namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface INotificationSendResultVisitor<T>
    {
        T Visit(NotificationSendResult.BadGateway result);
        T Visit(NotificationSendResult.InternalServerError result);
        T Visit(NotificationSendResult.Success result);
    }
}