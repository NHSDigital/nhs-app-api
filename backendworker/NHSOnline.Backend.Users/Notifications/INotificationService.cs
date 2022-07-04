using System.Threading.Tasks;
using NHSOnline.Backend.Users.Areas.Devices.Models;

namespace NHSOnline.Backend.Users.Notifications
{
    public interface INotificationService
    {
        Task<NotificationSendResult> Send(string nhsLoginId, NotificationSendRequest notificationSendRequest);
        Task<NotificationOutcomeResult> GetNotificationOutcomeDetails(string notificationId,string hubPath);
    }
}