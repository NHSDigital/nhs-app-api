namespace NHSOnline.Backend.Users.Notifications.Migration
{
    public interface IMigrationResultVisitor<out T>
    {
        T Visit(MigrationResult.Success result);
        T Visit(MigrationResult.BadGateway result);
        T Visit(MigrationResult.BadRequest result);
        T Visit(MigrationResult.InternalServerError result);
    }
}
