namespace NHSOnline.Backend.Worker.Router.Prescription
{
    public interface IPrescriptionResultVisitor<out T>
    {
        T Visit(GetPrescriptionsResult.SuccessfullyRetrieved result);
        T Visit(GetPrescriptionsResult.Unsuccessful result);
    }
}
