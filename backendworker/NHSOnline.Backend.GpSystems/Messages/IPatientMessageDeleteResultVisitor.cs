namespace NHSOnline.Backend.GpSystems.Messages
{
    public interface IPatientMessageDeleteResultVisitor<out T>
    {
        T Visit(DeletePatientMessageResult.Success result);

        T Visit(DeletePatientMessageResult.BadGateway result);

        T Visit(DeletePatientMessageResult.Forbidden result);

        T Visit(DeletePatientMessageResult.BadRequest result);

        T Visit(DeletePatientMessageResult.InternalServerError result);
    }
}