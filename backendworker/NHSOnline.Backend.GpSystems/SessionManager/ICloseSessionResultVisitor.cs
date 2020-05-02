namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public interface ICloseSessionResultVisitor<out T>
    {
        T Visit(CloseSessionResult.Success success);
        T Visit(CloseSessionResult.Failure failure);
    }
}