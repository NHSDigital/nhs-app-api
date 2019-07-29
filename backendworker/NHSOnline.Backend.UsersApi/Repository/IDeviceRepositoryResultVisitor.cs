namespace NHSOnline.Backend.UsersApi.Repository
{
    public interface IDeviceRepositoryResultVisitor<out T>
    {
        T Visit(DeviceRepositoryResult.Created result);
        T Visit(DeviceRepositoryResult.Failure result);
    }
}