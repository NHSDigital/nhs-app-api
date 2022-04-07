using Microsoft.Azure.NotificationHubs;
using NHSOnline.Backend.Users.Areas.Devices.Models;

namespace NHSOnline.Backend.Users.Notifications
{
    public interface IInstallationTemplateFactory
    {
         InstallationTemplate Create(DeviceType deviceType);
    }
}