using Microsoft.Azure.NotificationHubs;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface IInstallationFactory
    {
        Installation Create(string devicePns, DeviceType deviceType, string nhsLoginId);
    }
}