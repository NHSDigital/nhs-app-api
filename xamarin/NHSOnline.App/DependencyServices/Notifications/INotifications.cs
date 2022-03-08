using System.Threading.Tasks;

namespace NHSOnline.App.DependencyServices.Notifications
{
    public interface INotifications
    {
        Task<NotificationStatus> GetDeviceNotificationsStatus();
        Task<GetPnsTokenResult> GetPnsToken();
        bool NotificationServiceAvailable();
    }
}