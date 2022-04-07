using NHSOnline.Backend.Users.Notifications;

namespace NHSOnline.Backend.Users.Notifications
{
    public interface INotificationOutcomeResultVisitor<out T>
    {
        T Visit(NotificationOutcomeResult.BadGateway result);

        T Visit(NotificationOutcomeResult.InternalServerError result);

        T Visit(NotificationOutcomeResult.NotFound result);

        T Visit(NotificationOutcomeResult.Success result);

    }
}