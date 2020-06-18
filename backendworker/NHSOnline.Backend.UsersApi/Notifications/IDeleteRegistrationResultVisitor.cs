namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface IDeleteRegistrationResultVisitor<out T>
    {
        T Visit(DeleteRegistrationResult.Success result);
        T Visit(DeleteRegistrationResult.NotFound result);
        T Visit(DeleteRegistrationResult.BadGateway result);
        T Visit(DeleteRegistrationResult.InternalServerError result);
    }
}