using Microsoft.Azure.NotificationHubs;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface IInstallationTemplateFactory
    {
         InstallationTemplate Create(DeviceType deviceType);
    }
}