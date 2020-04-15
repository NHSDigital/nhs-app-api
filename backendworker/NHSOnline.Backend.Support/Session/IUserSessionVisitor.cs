namespace NHSOnline.Backend.Support.Session
{
    public interface IUserSessionVisitor<out TResult>
    {
        TResult Visit(P5UserSession userSession);

        TResult Visit(P9UserSession userSession);
    }
}