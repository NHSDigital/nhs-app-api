namespace NHSOnline.Backend.GpSystems.Session
{
    public interface ISessionExtendResultVisitor<out T>
    {
        T Visit(SessionExtendResult.Success result);

        T Visit(SessionExtendResult.BadGateway result);
    }
}