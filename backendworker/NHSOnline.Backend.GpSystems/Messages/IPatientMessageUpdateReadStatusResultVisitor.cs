namespace NHSOnline.Backend.GpSystems.Messages
{
    public interface IPatientMessageUpdateReadStatusResultVisitor<out T>
    {
        T Visit(PutPatientMessageReadStatusResult.Success result);

        T Visit(PutPatientMessageReadStatusResult.BadGateway result);

        T Visit(PutPatientMessageReadStatusResult.Forbidden result);

        T Visit(PutPatientMessageReadStatusResult.BadRequest result);

        T Visit(PutPatientMessageReadStatusResult.InternalServerError result);
    }
}