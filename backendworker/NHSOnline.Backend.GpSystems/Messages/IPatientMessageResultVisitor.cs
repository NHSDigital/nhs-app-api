namespace NHSOnline.Backend.GpSystems.Messages
{
    public interface IPatientMessageResultVisitor<out T>
    {
        T Visit(GetPatientMessageResult.Success result);

        T Visit(GetPatientMessageResult.BadGateway result);

        T Visit(GetPatientMessageResult.Forbidden result);

        T Visit(GetPatientMessageResult.BadRequest result);

        T Visit(GetPatientMessageResult.InternalServerError result);
    }
}