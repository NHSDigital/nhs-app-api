using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public interface IDeviceRegistrationResultVisitor<out T>
    {
        T Visit(DeviceRegistrationResult.Created result);
        T Visit(DeviceRegistrationResult.BadGateway result);
        T Visit(DeviceRegistrationResult.InternalServerError result);
    }
}