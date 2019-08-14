using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public interface IRegistrationResultAuditVisitor<out T>
    {
        T Visit(RegistrationResult.Success result);
        T Visit(RegistrationResult.BadGateway result);
        T Visit(RegistrationResult.InternalServerError result);
    }
}