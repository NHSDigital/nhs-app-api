using System.Threading.Tasks;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Users.Areas.Devices.Models;

namespace NHSOnline.Backend.Users.Registrations
{
    public interface INotificationsDecisionAuditService
    {
        Task LogAudit(NotificationsAuditData notificationsAuditData,
            AccessToken accessToken);
    }
}