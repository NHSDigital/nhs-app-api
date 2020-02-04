namespace NHSOnline.Backend.GpSystems.Messages
{
    public interface IPatientSendMessageResultVisitor<out T>
    {
        T Visit(PostPatientMessageResult.Success result);

        T Visit(PostPatientMessageResult.BadGateway result);

        T Visit(PostPatientMessageResult.Forbidden result);

        T Visit(PostPatientMessageResult.BadRequest result);

        T Visit(PostPatientMessageResult.InternalServerError result);
    }
}