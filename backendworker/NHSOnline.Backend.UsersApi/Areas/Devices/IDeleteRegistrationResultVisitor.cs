using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public interface IDeleteRegistrationResultVisitor<out T>
    {
        T Visit(DeleteRegistrationResult.Success result);
        T Visit(DeleteRegistrationResult.BadGateway result);
        T Visit(DeleteRegistrationResult.InternalServerError result);
    }
}