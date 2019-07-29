namespace NHSOnline.Backend.UsersApi.Azure
{
    public interface IRegistrationResultVisitor<out T>
    {
        T Visit(RegistrationResult.Success result);
        T Visit(RegistrationResult.Failure result);
    }
}