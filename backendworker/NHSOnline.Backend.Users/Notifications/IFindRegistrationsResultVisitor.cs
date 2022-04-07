namespace NHSOnline.Backend.Users.Notifications
{
    public interface IFindRegistrationsResultVisitor<out T>
    {
        T Visit(FindRegistrationsResult.Found result);
        T Visit(FindRegistrationsResult.NotFound result);
        T Visit(FindRegistrationsResult.BadGateway result);
        T Visit(FindRegistrationsResult.InternalServerError result);
    }
}