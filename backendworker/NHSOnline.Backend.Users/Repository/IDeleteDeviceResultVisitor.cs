namespace NHSOnline.Backend.Users.Repository
{
    public interface IDeleteDeviceResultVisitor<out T>
    {
        T Visit(DeleteDeviceResult.Success result);
        T Visit(DeleteDeviceResult.BadGateway result);
        T Visit(DeleteDeviceResult.InternalServerError result);
    }
}