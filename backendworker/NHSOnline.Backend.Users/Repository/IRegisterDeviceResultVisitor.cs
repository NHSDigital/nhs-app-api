namespace NHSOnline.Backend.Users.Repository
{
    public interface IRegisterDeviceResultVisitor<out T>
    {
        T Visit(RegisterDeviceResult.Created result);
        T Visit(RegisterDeviceResult.BadGateway result);
        T Visit(RegisterDeviceResult.InternalServerError result);
    }
}