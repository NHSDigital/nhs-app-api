using System.Threading.Tasks;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface INotificationService
    {
        Task<RegistrationResult> Register(NotificationRegistrationRequest request);
    }
}