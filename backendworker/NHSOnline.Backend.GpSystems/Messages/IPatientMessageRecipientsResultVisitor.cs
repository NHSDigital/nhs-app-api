namespace NHSOnline.Backend.GpSystems.Messages
{
    public interface IPatientMessageRecipientsResultVisitor<out T>
    {
        T Visit(GetPatientMessageRecipientsResult.Success result);

        T Visit(GetPatientMessageRecipientsResult.BadGateway result);

        T Visit(GetPatientMessageRecipientsResult.Forbidden result);

        T Visit(GetPatientMessageRecipientsResult.BadRequest result);

        T Visit(GetPatientMessageRecipientsResult.InternalServerError result);
    }
}