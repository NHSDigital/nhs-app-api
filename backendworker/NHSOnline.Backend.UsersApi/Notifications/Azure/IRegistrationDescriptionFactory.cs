using Microsoft.Azure.NotificationHubs;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Notifications.Azure
{
    public interface IRegistrationDescriptionFactory
    {
        RegistrationDescription Create(RegisterDeviceRequest registerDeviceRequest, string nhsLoginId);
    }
}