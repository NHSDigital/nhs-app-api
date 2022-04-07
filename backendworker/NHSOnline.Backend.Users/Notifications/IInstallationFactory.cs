using Microsoft.Azure.NotificationHubs;
using NHSOnline.Backend.Users.Notifications.Models;

namespace NHSOnline.Backend.Users.Notifications
{
    public interface IInstallationFactory
    {
        Installation Create(InstallationRequest request);
    }
}