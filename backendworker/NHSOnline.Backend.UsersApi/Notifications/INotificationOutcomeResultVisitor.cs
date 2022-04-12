namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface INotificationOutcomeResultVisitor<out T>
    {
        T Visit(NotificationOutcomeResult.BadGateway result);
        
        T Visit(NotificationOutcomeResult.InternalServerError result);
        
        T Visit(NotificationOutcomeResult.NotFound result);
        
        T Visit(NotificationOutcomeResult.Success result);
        
    }
}