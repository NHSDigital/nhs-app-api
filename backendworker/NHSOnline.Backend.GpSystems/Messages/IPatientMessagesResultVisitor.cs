namespace NHSOnline.Backend.GpSystems.Messages
{
    public interface IPatientMessagesResultVisitor<out T>
    {
        T Visit(GetPatientMessagesResult.Success result);

        T Visit(GetPatientMessagesResult.BadGateway result);

        T Visit(GetPatientMessagesResult.Forbidden result);

        T Visit(GetPatientMessagesResult.BadRequest result);

        T Visit(GetPatientMessagesResult.InternalServerError result);
    }
}