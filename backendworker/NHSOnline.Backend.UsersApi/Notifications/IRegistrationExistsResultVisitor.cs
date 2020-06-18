namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface IRegistrationExistsResultVisitor<out T>
    {
        T Visit(RegistrationExistsResult.Found result);
        T Visit(RegistrationExistsResult.NotFound result);
        T Visit(RegistrationExistsResult.BadGateway result);
        T Visit(RegistrationExistsResult.InternalServerError result);
    }
}