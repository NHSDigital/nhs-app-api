namespace NHSOnline.Backend.Auth.CitizenId
{
    public interface IRefreshTokenResultVisitor<T>
    {
        T Visit(RefreshAccessTokenResult.Success result);
        T Visit(RefreshAccessTokenResult.BadGateway result);
        T Visit(RefreshAccessTokenResult.InternalServerError result);
    }
}