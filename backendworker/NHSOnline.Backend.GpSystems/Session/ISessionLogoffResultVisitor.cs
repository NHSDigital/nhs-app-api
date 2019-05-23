namespace NHSOnline.Backend.GpSystems.Session
{
    public interface ISessionLogoffResultVisitor<out T>
    {
        T Visit(SessionLogoffResult.Success result);

        T Visit(SessionLogoffResult.Forbidden result);

        T Visit(SessionLogoffResult.BadGateway result);
    }
}