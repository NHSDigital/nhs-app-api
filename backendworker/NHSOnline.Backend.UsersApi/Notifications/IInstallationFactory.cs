using Microsoft.Azure.NotificationHubs;
using NHSOnline.Backend.UsersApi.Notifications.Models;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface IInstallationFactory
    {
        Installation Create(InstallationRequest request);
    }
}