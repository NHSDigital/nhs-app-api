namespace NHSOnline.Backend.UsersApi.Repository
{
    public interface ISearchDeviceResultVisitor<out T>
    {
        T Visit(SearchDeviceResult.Found result);
        T Visit(SearchDeviceResult.NotFound result);
        T Visit(SearchDeviceResult.BadGateway result);
        T Visit(SearchDeviceResult.InternalServerError result);
    }
}