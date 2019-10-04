namespace NHSOnline.Backend.UserInfoApi.Areas.UserInfo
{
    public interface IInfoResultVisitor<out T>
    {
        T Visit(PostInfoResult.Created result);
        T Visit(PostInfoResult.BadGateway result);
        T Visit(PostInfoResult.InternalServerError result);
    }
}