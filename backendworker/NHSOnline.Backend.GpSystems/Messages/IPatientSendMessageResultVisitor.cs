namespace NHSOnline.Backend.GpSystems.Messages
{
    public interface IPatientSendMessageResultVisitor<out T>
    {
        T Visit(PostSendMessageResult.Success result);

        T Visit(PostSendMessageResult.BadGateway result);

        T Visit(PostSendMessageResult.Forbidden result);

        T Visit(PostSendMessageResult.BadRequest result);

        T Visit(PostSendMessageResult.InternalServerError result);
    }
}