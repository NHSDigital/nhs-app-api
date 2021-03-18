using System.Threading.Tasks;

namespace NHSOnline.App.DependencyServices.Notifications
{
    public interface INotifications
    {
        NotificationStatus GetDeviceNotificationsStatus();
        Task<GetPnsTokenResult> GetPnsToken();
    }
}