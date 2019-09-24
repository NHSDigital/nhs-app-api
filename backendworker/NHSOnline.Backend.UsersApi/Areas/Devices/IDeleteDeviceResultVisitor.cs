using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public interface IDeleteDeviceResultVisitor<out T>
    {
        T Visit(DeleteDeviceResult.Success result);
        T Visit(DeleteDeviceResult.BadGateway result);
        T Visit(DeleteDeviceResult.InternalServerError result);
    }
}