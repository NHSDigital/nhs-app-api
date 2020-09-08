using System.Threading.Tasks;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface INotificationService
    {
        Task<NotificationSendResult> Send(string nhsLoginId, NotificationSendRequest notificationSendRequest);
    }
}