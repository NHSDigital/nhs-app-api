namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface IRegistrationResultVisitor<out T>
    {
        T Visit(RegistrationResult.Success result);
        T Visit(RegistrationResult.BadGateway result);
        T Visit(RegistrationResult.InternalServerError result);
    }
}