namespace NHSOnline.Backend.GpSystems.Session
{
    public interface IGpSessionCreateResultVisitor<out T>
    {
        T Visit(GpSessionCreateResult.Success result);

        T Visit(GpSessionCreateResult.Forbidden result);

        T Visit(GpSessionCreateResult.BadGateway result);

        T Visit(GpSessionCreateResult.InternalServerError result);

        T Visit(GpSessionCreateResult.BadRequest result);
    }
}