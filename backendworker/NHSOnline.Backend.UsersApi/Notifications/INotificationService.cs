using System.Threading.Tasks;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications.Models;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface INotificationService
    {
        Task<NotificationSendResult> Send(string nhsLoginId, NotificationSendRequest notificationSendRequest);
        Task<NotificationOutcomeResult> GetNotificationOutcomeDetails(string notificationId,string hubPath);
    }
}