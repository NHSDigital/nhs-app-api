namespace NHSOnline.Backend.UserInfoApi.Areas.UserInfo
{
    public interface IGetInfoResultVisitor<out T>
    {
        T Visit(GetInfoResult.Found result);
        T Visit(GetInfoResult.NotFound result);
        T Visit(GetInfoResult.BadGateway result);
        T Visit(GetInfoResult.InternalServerError result);
    }
}