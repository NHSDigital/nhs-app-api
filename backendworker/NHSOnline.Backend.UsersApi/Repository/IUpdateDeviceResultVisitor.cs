namespace NHSOnline.Backend.UsersApi.Repository
{
    public interface IUpdateDeviceResultVisitor<out T>
    {
        T Visit(UpdateDeviceResult.Updated result);
        T Visit(UpdateDeviceResult.NotFound result);
        T Visit(UpdateDeviceResult.InternalServerError result);
    }
}